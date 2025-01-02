import { Component, OnInit, ViewChild } from '@angular/core';
import { OrderListDTO } from '../../../models/response/orders/order';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatDialog, MatDialogActions, MatDialogClose, MatDialogContent } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from '../../confirmation-dialog/confirmation-dialog.component';
import { MatSort, Sort } from '@angular/material/sort';
import { OrderService } from '../../../services/order/order.service';

const ORDERS: OrderListDTO[] = [
  { orderDate: '2023-12-01', orderStatus: 'Delivered', netAmount: 100.5, itemCount: 2, kitchenName: "Enzo's Takeaway" },
  { orderDate: '2023-12-02', orderStatus: 'Pending', netAmount: 200.0, itemCount: 4, kitchenName: "Enzp's Takeaway" },
  // Add more mock orders here
];

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.css'
})

export class OrdersComponent implements OnInit{
  displayedColumns: string[] = ['orderDate', 'orderStatus', 'netAmount', 'itemCount', 'actions'];
  dataSource = new MatTableDataSource<OrderListDTO>([]);
  orders: OrderListDTO[] = [];
  totalRecords: number = 0;
  pageSize = 10; // Default page size
  currentPage = 0; // Current page index
  orderStatus: string = '';
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private dialog: MatDialog, private orderService: OrderService) {}

  ngOnInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.loadOrders();
  }
  
  loadOrders(){
    this.orderService.viewAllOrders(this.pageSize, 1, this.orderStatus).subscribe((res) => {
      this.dataSource.data = res.orders;
      this.totalRecords = res.totalCount;
      console.log(res);
    })
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
    this.currentPage = event.pageIndex;
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
        // Add logic to handle the action (e.g., API call)
      } else {
        console.log(`${action} cancelled for order:`, order);
      }
    });
  }
}
