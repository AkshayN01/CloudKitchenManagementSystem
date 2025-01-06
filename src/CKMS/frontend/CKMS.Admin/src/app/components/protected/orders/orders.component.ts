import { Component, OnInit, ViewChild } from '@angular/core';
import { OrderListDTO } from '../../../models/response/orders/order';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatDialog, MatDialogActions, MatDialogClose, MatDialogContent } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from '../../confirmation-dialog/confirmation-dialog.component';
import { MatSort, Sort } from '@angular/material/sort';
import { OrderService } from '../../../services/order/order.service';
import { UtilityService } from '../../../services/utility/utility.service';
import { NotificationService } from '../../../services/notification/notification.service';
import { DatePipe } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.css',
  providers: [DatePipe]
})

export class OrdersComponent implements OnInit{
  displayedColumns: string[] = ['orderDate', 'orderStatus', 'netAmount', 'itemCount', 'actions'];
  dataSource = new MatTableDataSource<OrderListDTO>([]);
  orders: OrderListDTO[] = [];
  totalRecords: number = 0;
  pageSize = 10; // Default page size
  currentPage = 1; // Current page index
  orderStatus: string = '';
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private dialog: MatDialog, private orderService: OrderService, private router: Router,
    private utilityService: UtilityService, private notificationService: NotificationService, private datePipe: DatePipe) {}

  ngOnInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.loadOrders();
    this.notificationService.messages$.subscribe(messages => {
      this.currentPage = 1;
      this.loadOrders();
    });
  }
  
  loadOrders(){
    this.orderService.viewAllOrders(this.pageSize, this.currentPage, this.orderStatus).subscribe((res) => {
      this.dataSource.data = res.orders;
      this.totalRecords = res.totalCount;
      console.log(res);
    })
  }

  formatDate(dateString: string): string {
    return this.datePipe.transform(dateString, 'MMM d, y, h:mm a') || '';
  }

  onSortChange(event: any) {
    console.log('Sort changed:', event);
    if (event.active === 'orderStatus' && event.direction) {
      // this.fetchSortedData(event.direction);
    }
  }

  // Handle page changes
  onPageChange(event: PageEvent) {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex + 1;
    this.loadOrders(); // Reload data for new page
  }
  
  openConfirmationDialog(action: string, order: OrderListDTO): void {
    console.log(action)
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '250px',
      data: {
        title: `Confirm ${action}`,
        message: `Are you sure you want to ${action} this order?`
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log(`${action} confirmed for order:`, order);
        
        this.orderService.updateOrder(action, order.orderId).subscribe((res) => {
          this.loadOrders();
          this.utilityService.openSnackBar("order successfully " + action);
        })
      } else {
        console.log(`${action} cancelled for order:`, order);
      }
    });
  }

  viewOrder(orderId:string){
    this.router.navigate(['/order-detail', orderId]);
  }
}
