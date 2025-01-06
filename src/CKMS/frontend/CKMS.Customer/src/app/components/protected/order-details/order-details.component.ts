import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../../services/order/order.service';
import { OrderDTO } from '../../../models/response/order';
import { UtilityService } from '../../../services/utility/utility.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-order-details',
  templateUrl: './order-details.component.html',
  styleUrl: './order-details.component.css'
})
export class OrderDetailsComponent implements OnInit{
  order!: OrderDTO;

  displayedColumns: string[] = ['itemName', 'quantity'];
  progressPercentage: number = 0;

  constructor(private orderService: OrderService, private route: ActivatedRoute, private utilityService: UtilityService){}

  ngOnInit(): void {
    const orderId = this.route.snapshot.paramMap.get('id');
    console.log(orderId);
    if (orderId) {
      this.fetchOrderDetails(orderId);
    }
  }

  fetchOrderDetails(orderId: string): void {
    this.orderService.viewOrder(orderId).subscribe((res) => {
      this.order = res;
      this.calculateProgress();
    });
  }

  calculateProgress(): void {
    const steps = [
      this.order.orderDate,
      this.order.inProgressDate,
      this.order.outForDeliveryDate,
      this.order.deliveredDate
    ];

    const completedSteps = steps.filter((step) => step).length;
    this.progressPercentage = (completedSteps / steps.length) * 100;
  }
}
