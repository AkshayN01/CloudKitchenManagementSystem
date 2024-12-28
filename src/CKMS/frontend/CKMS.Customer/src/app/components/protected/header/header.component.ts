import { Component, OnInit } from '@angular/core';
import { SessionService } from '../../../services/session/session.service';
import { WebNoti } from '../../../models/response/notification';
import { NotificationService } from '../../../services/notification/notification.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit{
  username:string|null = '';
  constructor(private notificationService: NotificationService, private sessionService: SessionService)
  {
  }

  ngOnInit(): void {
    this.username = this.sessionService.getUserName();
    this.notificationService.messages$.subscribe(messages => {
      this.notifications = messages;
    });
  }

  notifications: WebNoti[] = [];

  logout() {
    console.log('Logout clicked');
    this.sessionService.logout();
  }
}
