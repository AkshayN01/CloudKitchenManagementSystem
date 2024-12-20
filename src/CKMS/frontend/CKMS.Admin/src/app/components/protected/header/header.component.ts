import { Component } from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  username = 'John Doe';
  notifications = [
    'New message from Alex',
    'Your report is ready',
    'Meeting at 3 PM'
  ];

  logout() {
    console.log('Logout clicked');
    // Add logout logic here
  }
}
