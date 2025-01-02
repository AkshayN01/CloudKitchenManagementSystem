import { Component, OnInit } from '@angular/core';
import { WebNoti } from '../../../models/response/notification/web-notification';
import { NotificationService } from '../../../services/notification/notification.service';
import { SessionService } from '../../../services/session/session.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit{
  username:string|null = '';
  kitchenName: string|null = '';
  constructor(private notificationService: NotificationService, private sessionService: SessionService, private router: Router)
  {
  }

  ngOnInit(): void {
    this.username = this.sessionService.getUserName();
    this.kitchenName = this.sessionService.getKitchenName();
    this.notificationService.messages$.subscribe(messages => {
      this.notifications = messages;
    });
  }
  // notifications = [
  //   'New message from Alex',
  //   'Your report is ready',
  //   'Meeting at 3 PM'
  // ];
  notifications: WebNoti[] = [];

  logout() {
    console.log('Logout clicked');
    // Add logout logic here
    this.sessionService.logout();
    this.notificationService.stopConnection();
    this.router.navigate(['/login'])
  }
}
