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
  unreadCount: number = 0;
  username:string|null = '';
  kitchenName: string|null = '';
  notifications: WebNoti[] = [];
  readNotifications: WebNoti[] = [];
  constructor(private notificationService: NotificationService, private sessionService: SessionService, private router: Router)
  {
  }

  ngOnInit(): void {
    this.username = this.sessionService.getUserName();
    this.kitchenName = this.sessionService.getKitchenName();
    this.notificationService.messages$.subscribe(messages => {
      this.notifications = messages;
      if(this.readNotifications.length == 0){
        this.unreadCount = this.notifications.length;
      }
      else{
        console.log(this.readNotifications)
        var newNotis = this.notifications.filter(
          (item2) => !this.readNotifications.some((item1) => item1.title === item2.title && item1.body === item2.body)
        );
        console.log(newNotis);
        this.unreadCount = newNotis.length;
        console.log(this.unreadCount)
      }
      console.log(messages);
    });
  }

  get hasNewNotifications(): boolean {
    return this.unreadCount > 0;
  }

  markNotificationsAsRead(): void {
    this.notifications.forEach((item) => {
      this.readNotifications.push(item);
    })
    console.log(this.readNotifications.length)
    this.unreadCount = 0;
  }

  logout() {
    console.log('Logout clicked');
    // Add logout logic here
    this.sessionService.logout();
    this.notificationService.stopConnection();
    this.router.navigate(['/login'])
  }
}
