import { Component, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { TopCustomers } from '../../../models/response/orders/report';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { ReportService } from '../../../services/report/report.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-customers',
  templateUrl: './customers.component.html',
  styleUrl: './customers.component.css'
})
export class CustomersComponent {
  displayedColumns: string[] = ['index', 'customerName', 'totalOrders', 'actions'];
  dataSource: MatTableDataSource<TopCustomers> = new MatTableDataSource<TopCustomers>([]);
  
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  pageSize: number = 10; // Default page size
  pageNumber: number = 1; // Default page number
  totalRecords: number = 0; // Total number of records from the API

  constructor(private reportsService: ReportService, private router: Router) {}

  ngOnInit(): void {
    this.fetchTopCustomers();
  }

  fetchTopCustomers(): void {
    this.reportsService.getTopCustomers('', '', this.pageSize, this.pageNumber, true).subscribe(customers => {
      this.dataSource.data = customers;
      this.dataSource.paginator = this.paginator; // Set paginator after data load
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.pageNumber = event.pageIndex + 1; // API uses 1-based index
    this.fetchTopCustomers(); // Fetch new data for the current page
  }

  viewCustomer(customerId: string): void {
    this.router.navigate(['/customer-summary', customerId]);
  }
}
