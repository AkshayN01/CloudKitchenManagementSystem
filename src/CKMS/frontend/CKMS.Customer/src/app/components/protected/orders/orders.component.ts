import { Component, OnInit } from '@angular/core';
import { OrderListDTO } from '../../../models/response/order';
import { OrderService } from '../../../services/order/order.service';
import { Router } from '@angular/router';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.css'
})
export class OrdersComponent implements OnInit{
  orders: OrderListDTO[] = [];
  totalCount: number = 0;

  // Pagination variables
  pageSize: number = 10;
  pageIndex: number = 1;

  constructor(private orderService: OrderService, private router: Router) {}

  ngOnInit(): void {
    this.fetchOrders();
  }

  fetchOrders(): void {
    const params = {
      pageIndex: this.pageIndex.toString(),
      pageSize: this.pageSize.toString(),
    };

    this.orderService.viewAllOrders(this.pageSize, this.pageIndex).subscribe((res) => {
      this.orders = res.orders;
      this.totalCount = res.totalCount;
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.fetchOrders();
  }

  viewOrderSummary(orderId: string): void {
    // Navigate to the order-summary page
    // Replace '/order-summary' with the actual route
    this.router.navigate(['/order-summary', orderId]);
  }
}
