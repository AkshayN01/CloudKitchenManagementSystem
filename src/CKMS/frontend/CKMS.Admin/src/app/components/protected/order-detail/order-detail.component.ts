import { Component, OnInit } from '@angular/core';
import { OrderDetail, OrderItem } from '../../../models/response/orders/order';
import { OrderService } from '../../../services/order/order.service';
import { ActivatedRoute } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-order-detail',
  templateUrl: './order-detail.component.html',
  styleUrl: './order-detail.component.css'
})
export class OrderDetailComponent implements OnInit{
  orderDetail: OrderDetail | undefined;
  selectedStatus: string = '';
  dataSource = new MatTableDataSource<OrderItem>([]);

  constructor(private orderService: OrderService, private route: ActivatedRoute, private snackBar: MatSnackBar) {}

  ngOnInit(): void {
    const orderId = this.route.snapshot.paramMap.get('id');
    if (orderId) {
      this.fetchOrderDetails(orderId);
    }
  }

  fetchOrderDetails(orderId: string): void {
    this.orderService.viewOrder(orderId).subscribe((res) => {
      this.orderDetail = res;
      this.selectedStatus = res.status;
      this.dataSource.data = res.items;
    });
  }

  updateOrderStatus(): void {
    if(this.orderDetail){
      const updatedStatus = this.selectedStatus;
      this.orderService.updateOrder(updatedStatus, this.orderDetail.orderId).subscribe((res) => {
        if(res){
          this.snackBar.open('Order status updated successfully!', 'Close', { duration: 3000 });
          this.orderDetail!.status = updatedStatus;
        }
        else{
          console.error('Failed to update order status:', error);
          this.snackBar.open('Failed to update order status.', 'Close', { duration: 3000 });
        }
      })
    }
  }
}
