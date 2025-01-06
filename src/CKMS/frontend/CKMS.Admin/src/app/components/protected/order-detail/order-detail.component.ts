import { Component, OnInit } from '@angular/core';
import { OrderDetail, OrderItem } from '../../../models/response/orders/order';
import { OrderService } from '../../../services/order/order.service';
import { ActivatedRoute } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmationDialogComponent } from '../../confirmation-dialog/confirmation-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { Chart, registerables } from 'chart.js';
import { ChartType } from 'angular-google-charts';

interface StatusOption{
  name: string,
  value: string,
}
interface OrderStage {
  stageName: string;
  startDate: string | Date;
  endDate: string | Date;
}
@Component({
  selector: 'app-order-detail',
  templateUrl: './order-detail.component.html',
  styleUrl: './order-detail.component.css'
})
export class OrderDetailComponent implements OnInit{
  orderDetail: OrderDetail | undefined;
  selectedStatus: string = '';
  dataSource = new MatTableDataSource<OrderItem>([]);
  statusOptionVal: StatusOption[] = [];
  orderStages: OrderStage[] = [];
  chartType: ChartType = ChartType.Timeline;
  chartData: any[] = [];
  chartColumns = ['Task Name', 'Start Date', 'End Date'];
  chartOptions = {
    height: 400,
    timeline: {
      showRowLabels: true, // Show row labels
      barLabelStyle: { fontSize: 12 }, // Customize bar label font size
      rowLabelStyle: { fontSize: 14 }, // Customize row label font size
    },
    hAxis: {
      format: 'MMM dd, yyyy', // Customize date format on the axis
      textStyle: { fontSize: 12 } // Horizontal axis text style
    },
    width: 700 // Increase overall chart width
  };
  

  constructor(private dialog: MatDialog, private orderService: OrderService, private route: ActivatedRoute, private snackBar: MatSnackBar) 
  {
      Chart.register(...registerables);
  }

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
      this.getOptionValue(res.status);
      if(res.status == 'delivered'){
      this.generateOrderStage();
      }
      // this.prepareChartData();
    });
  }

  generateOrderStage(){
    this.chartData = [
      ['Order Placed', new Date(this.orderDetail!.orderDate), new Date(this.orderDetail!.inProgressDate)],
      ['In Progress', new Date(this.orderDetail!.inProgressDate), new Date(this.orderDetail!.outForDeliveryDate)],
      ['Out for Delivery', new Date(this.orderDetail!.outForDeliveryDate), new Date(this.orderDetail!.deliveredDate)],
      // ['Delivered', new Date(this.orderDetail!.deliveredDate), new Date(this.orderDetail!.deliveredDate)]
    ];
  }

  getOptionValue(orderStatus: string){
    this.statusOptionVal = [];
    if(orderStatus == 'placed'){
      var statusOption1: StatusOption = { name: 'Accepted', value: 'accepted' };
      var statusOption2: StatusOption = { name: 'Declined', value: 'declined' };
      this.statusOptionVal.push(statusOption1, statusOption2);
    }
    else if(orderStatus == 'declined'){
      var statusOption1: StatusOption = { name: 'Accepted', value: 'accepted' };
      this.statusOptionVal.push(statusOption1);
    }
    else if(orderStatus == 'accepted'){
      var statusOption1: StatusOption = { name: 'In Progress', value: 'inprogress' };
      var statusOption2: StatusOption = { name: 'Declined', value: 'declined' };
      this.statusOptionVal.push(statusOption1, statusOption2);
    }
    else if(orderStatus == 'inprogress'){
      var statusOption1: StatusOption = { name: 'Out For Delivery', value: 'outfordelivery' };
      var statusOption2: StatusOption = { name: 'Declined', value: 'declined' };
      this.statusOptionVal.push(statusOption1, statusOption2);
    }
    else if(orderStatus == 'outfordelivery'){
      var statusOption1: StatusOption = { name: 'Delivered', value: 'delivered' };
      var statusOption2: StatusOption = { name: 'Declined', value: 'declined' };
      this.statusOptionVal.push(statusOption1, statusOption2);
    }
  }

  updateOrderStatus(): void {
    if(this.orderDetail){
      const updatedStatus = this.selectedStatus;
      const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
            width: '250px',
            data: {
              title: `Confirm ${updatedStatus}`,
              message: `Are you sure you want to update the status to ${updatedStatus}?`
            }
          });
      
          dialogRef.afterClosed().subscribe(result => {
            if (result) {
              console.log(`${updatedStatus} confirmed for order:`);
              
              this.orderService.updateOrder(updatedStatus, this.orderDetail!.orderId).subscribe((res) => {
                if(res){
                  this.snackBar.open('Order status updated successfully!', 'Close', { duration: 3000 });
                  this.orderDetail!.status = updatedStatus;
                  this.selectedStatus = updatedStatus;
                  this.fetchOrderDetails(this.orderDetail!.orderId);
                  this.getOptionValue(updatedStatus);
                }
                else{
                  console.error('Failed to update order status:');
                  this.snackBar.open('Failed to update order status.', 'Close', { duration: 3000 });
                }
              })
            } else {
              console.log(`${updatedStatus} cancelled for order:`);
            }
          });
      
    }
  }
}
