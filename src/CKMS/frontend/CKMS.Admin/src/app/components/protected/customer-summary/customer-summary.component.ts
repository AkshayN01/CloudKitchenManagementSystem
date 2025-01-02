import { Component, OnInit } from '@angular/core';
import { CustomerSummary, LatestOrder } from '../../../models/response/orders/report';
import {
  Chart,
  DoughnutController,
  ArcElement,
  Tooltip,
  Legend,
} from 'chart.js';
import { ReportService } from '../../../services/report/report.service';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-customer-summary',
  templateUrl: './customer-summary.component.html',
  styleUrl: './customer-summary.component.css',
  providers: [DatePipe]
})
export class CustomerSummaryComponent implements OnInit{
  customer!: CustomerSummary;
  displayedColumns: string[] = ['orderDate', 'status', 'itemCount', 'netAmount'];
  expandedOrder: any = null;

  chart: any;

  constructor(private reportService: ReportService, private route: ActivatedRoute, private datePipe: DatePipe)
  {
    Chart.register(DoughnutController, ArcElement, Tooltip, Legend);
  }
  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    console.log(id);
    if(id != null){
      this.reportService.getCustomerSummary(id).subscribe((res) => {
        this.customer = res;
        this.createChart();
      })
    };
  }

  formatDate(dateString: string): string {
    return this.datePipe.transform(dateString, 'MMM d, y, h:mm a') || '';
  }

  createChart(): void {
    const labels = this.customer.orderingPatterns.map(op => op.timePeriod);
    const data = this.customer.orderingPatterns.map(op => op.ordersCount);

    this.chart = new Chart('orderPatternChart', {
      type: 'doughnut', // Set chart type to 'doughnut'
      data: {
        labels: labels,
        datasets: [{
          data: data,
          backgroundColor: ['#FFA726', '#66BB6A', '#42A5F5', '#AB47BC'], // Colors for each slice
          borderWidth: 1
        }]
      },
      options: {
        responsive: true,
        plugins: {
          legend: { 
            display: true,
            position: 'top' // Position of the legend
          },
          tooltip: { enabled: true }
        }
      }
    });
  }

  toggleRow(order: any): void {
    this.expandedOrder = this.expandedOrder === order ? null : order;
  }
}
