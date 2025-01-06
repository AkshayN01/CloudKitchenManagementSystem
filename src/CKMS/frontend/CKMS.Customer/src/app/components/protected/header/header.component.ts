import { Component, OnInit } from '@angular/core';
import { SessionService } from '../../../services/session/session.service';
import { WebNoti } from '../../../models/response/notification';
import { NotificationService } from '../../../services/notification/notification.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit{
  unreadCount: number = 0;
  username:string|null = '';
  notificationCount!: number;
  notifications: WebNoti[] = [];
  readNotifications: WebNoti[] = [];
  constructor(private notificationService: NotificationService, private sessionService: SessionService, private router: Router)
  {
  }

  ngOnInit(): void {
    this.username = this.sessionService.getUserName();
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
    });
  }

  get hasNewNotifications(): boolean {
    return this.unreadCount > 0;
  }

  markNotificationsAsRead(): void {
    this.notifications.forEach((item) => {
      this.readNotifications.push(item);
    })
    this.unreadCount = 0;
  }


  logout() {
    console.log('Logout clicked');
    this.sessionService.logout();
    this.notificationService.stopConnection();
    this.router.navigate(['/login']);
  }

  // Method called when the menu is opened
  onNotificationMenuOpened(): void {
    this.notificationCount = 0; // Reset the count
    this.notifications.forEach((x) => x.isViewed = true );
  }
}
