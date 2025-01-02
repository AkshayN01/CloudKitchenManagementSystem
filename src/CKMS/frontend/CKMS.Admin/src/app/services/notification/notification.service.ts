import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { WebNoti } from '../../models/response/notification/web-notification';
import { environment } from '../../../environments/environment';
import { IHttpConnectionOptions } from '@microsoft/signalr';
import { SessionService } from '../session/session.service';
import { Router } from '@angular/router';
import { UtilityService } from '../utility/utility.service';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  private hubConnection!: signalR.HubConnection;
  
  private messagesSubject = new BehaviorSubject<WebNoti[]>([]);
  public messages$ = this.messagesSubject.asObservable();
  
  private notificationHubUrl = environment.notificationAdminUrl;

  constructor(private router: Router, private sessionService: SessionService, private utilityService: UtilityService) 
  {
    this.createConnection();
    this.startConnection();
    this.startNotificationSetup();
  }

  private createConnection() {
    var token = this.sessionService.getToken() ?? '';
    const options: IHttpConnectionOptions = {
      accessTokenFactory: () => {
        return token;
      }
    };
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.notificationHubUrl, options)
      .build();
  }

  private startConnection() {
    if (this.sessionService.isTokenExpired()) {
      this.sessionService.logout();
      this.router.navigate(['/login']);
      this.utilityService.openSnackBar('Token has expired. Please login again');
      return;
    }
    if (this.hubConnection.state === signalR.HubConnectionState.Disconnected) {
      this.hubConnection.start()
        .then(() => console.log('Connection started'))
        .catch(err => {
          console.log('Error while starting connection: ' + err);
          setTimeout(() => this.startConnection(), 5000); // Retry connection after 5 seconds
        });
    }
  
    this.hubConnection.onreconnecting(error => {
      console.log(`Reconnecting: ${error}`);
    });
  
    this.hubConnection.onreconnected(connectionId => {
      console.log(`Reconnected: ${connectionId}`);
    });
  
    this.hubConnection.onclose(error => {
      console.log(`Connection closed: ${error}`);
      setTimeout(() => this.startConnection(), 5000); // Retry connection after 5 seconds
    });
  }

  private startNotificationSetup(){
    this.hubConnection.on('ReceiveNotification', (title, message) => {
      var notiBody: WebNoti= { body: message, title: title };
      const notifications = this.messagesSubject.value;
      notifications.push(notiBody);
      this.messagesSubject.next(notifications);
    });
  }
  
  public stopConnection() {
    if (this.hubConnection.state !== signalR.HubConnectionState.Disconnected) {
      this.hubConnection.stop()
        .then(() => console.log('Connection stopped'))
        .catch(err => console.log('Error while stopping connection: ' + err));
    }
  }
}
