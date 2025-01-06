import { Component, OnInit } from '@angular/core';
import { OrderListDTO } from '../../../models/response/order';
import { OrderService } from '../../../services/order/order.service';
import { Router } from '@angular/router';
import { PageEvent } from '@angular/material/paginator';
import { NotificationService } from '../../../services/notification/notification.service';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.css'
})
export class OrdersComponent implements OnInit{
  orders: OrderListDTO[] = [];
  totalOrders = 0;
  pageSize = 5;
  currentPage = 1;

  constructor(private orderService: OrderService, private router: Router, private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.notificationService.messages$.subscribe(messages => {
      this.currentPage = 1;
      this.fetchOrders();
    });
    this.fetchOrders();
  }

  fetchOrders(): void {
    const params = {
      pageIndex: this.currentPage.toString(),
      pageSize: this.pageSize.toString(),
    };

    this.orderService.viewAllOrders(this.pageSize, this.currentPage).subscribe((res) => {
      this.orders = res.orders;
      this.totalOrders = res.totalCount;
    });
  }

  onPageChange(event: PageEvent): void {
    this.currentPage = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.fetchOrders();
  }

  viewOrderSummary(orderId: string): void {
    // Navigate to the order-summary page
    // Replace '/order-summary' with the actual route
    this.router.navigate(['/order-detail', orderId]);
  }
  formatDate(dateString: string): string {
    const options: Intl.DateTimeFormatOptions = { year: 'numeric', month: 'short', day: '2-digit' };
    return new Date(dateString).toLocaleDateString('en-US', options);
  }
  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'accepted': return 'status-accepted';
      case 'declined': return 'status-declined';
      case 'cancelled': return 'status-cancelled';
      case 'outfordelivery': return 'status-outfordelivery';
      case 'inprogress': return 'status-inprogress';
      case 'delivered': return 'status-delivered';
      default: return '';
    }
  }
}
