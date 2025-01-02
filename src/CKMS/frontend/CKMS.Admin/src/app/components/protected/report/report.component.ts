import { Component } from '@angular/core';
import { ReportService } from '../../../services/report/report.service';
import { BestSellingDish, OrderReportSummary, TopCustomers } from '../../../models/response/orders/report';
import { Chart, registerables, DoughnutController, ArcElement, Tooltip, Legend } from 'chart.js';
import { UtilityService } from '../../../services/utility/utility.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrl: './report.component.css'
})
export class ReportComponent {
  orderSummary!: OrderReportSummary;
  bestSellingDishes: BestSellingDish[] = [];
  leastSellingDishes: BestSellingDish[] = [];
  topCustomers: TopCustomers[] = [];

  startDate: string;
  endDate: string;

  bestSellingChart: Chart | null = null;
  leastSellingChart: Chart | null = null;
  topCustomersChart: Chart | null = null;
  orderingPatternChart: Chart | null = null;

  constructor(private reportsService: ReportService, private utilityService: UtilityService, private router: Router) {
    // Default values for start and end date
    const today = new Date();
    // this.endDate = today.toISOString().split('T')[0];
    // this.startDate = new Date(today.setDate(today.getDate() - 30)).toISOString().split('T')[0];
    this.startDate = '2024-01-01'; // ISO format for 01/01/2024
    this.endDate = '2024-11-30'; // ISO format for 30/11/2024
    Chart.register(...registerables);
    Chart.register(DoughnutController, ArcElement, Tooltip, Legend);
  }

  ngOnInit(): void {
    this.fetchData();
  }

  fetchData(): void {
    var sDate = this.utilityService.convertDate(this.startDate);
    var eDate = this.utilityService.convertDate(this.endDate);
    this.reportsService.getOrderReportSummary(sDate, eDate).subscribe(summary => {
      this.orderSummary = summary;
      this.createOrderingPatternChart();
    });

    this.reportsService.getBestSellingDishes(sDate, eDate, 6, true).subscribe(dishes => {
      this.bestSellingDishes = dishes;
      this.createBestSellingDishesChart();
    });

    
    this.reportsService.getBestSellingDishes(sDate, eDate, 6, false).subscribe(dishes => {
      this.leastSellingDishes = dishes;
      this.createLeastSellingDishesChart();
    });

    this.reportsService.getTopCustomers(sDate, eDate, 5, 0, true).subscribe(customers => {
      this.topCustomers = customers;
      this.createTopCustomersChart();
    });
  }

  createOrderingPatternChart(): void {
    if(this.orderingPatternChart)
      this.orderingPatternChart.destroy();
    const labels = this.orderSummary.orderingPatterns.map(op => op.timePeriod);
    const data = this.orderSummary.orderingPatterns.map(op => op.ordersCount);

    this.orderingPatternChart = new Chart('orderPatternChart', {
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

  createBestSellingDishesChart(): void {
    if(this.bestSellingChart)
      this.bestSellingChart.destroy();

    const labels = this.bestSellingDishes.map(dish => dish.menuItemName);
    const data = this.bestSellingDishes.map(dish => dish.orderCount);

    this.bestSellingChart = new Chart('bestSellingDishesChart', {
      type: 'bar',
    data: {
      labels: labels,
      datasets: [{
        label: 'Order Count',
        data: data,
        backgroundColor: '#42A5F5',
      }]
    },
    options: {
      responsive: true,
      plugins: {
        legend: { display: false },
      },
      scales: {
        x: { stacked: true, title: { display: true, text: 'Dishes' } },
        y: { stacked: true, title: { display: true, text: 'Order Count' }, beginAtZero: true }
      }
    }
    });
  }

  createLeastSellingDishesChart(): void {
    if(this.leastSellingChart)
      this.leastSellingChart.destroy();

    const labels = this.leastSellingDishes.map(dish => dish.menuItemName);
    const data = this.leastSellingDishes.map(dish => dish.orderCount);

    this.leastSellingChart = new Chart('leastSellingDishesChart', {
      type: 'bar',
    data: {
      labels: labels,
      datasets: [{
        label: 'Order Count',
        data: data,
        backgroundColor: '#C30010',
      }]
    },
    options: {
      responsive: true,
      plugins: {
        legend: { display: false },
      },
      scales: {
        x: { stacked: true, title: { display: true, text: 'Dishes' } },
        y: { stacked: true, title: { display: true, text: 'Order Count' }, beginAtZero: true }
      }
    }
    });
  }

  createTopCustomersChart(): void {
    if(this.topCustomersChart)
      this.topCustomersChart.destroy();
    const labels = this.topCustomers.map(customer => customer.customerName);
    const data = this.topCustomers.map(customer => customer.totalOrders);

    this.topCustomersChart = new Chart('topCustomersChart', {
      type: 'bar',
      data: {
        labels: labels,
        datasets: [{
          label: 'Total Orders',
          data: data,
          backgroundColor: '#66BB6A',
        }]
      },
      options: {
        indexAxis: 'y', // Horizontal bar chart
        responsive: true,
        plugins: {
          legend: { display: false },
        },
        scales: {
          x: { title: { display: true, text: 'Total Orders' }, beginAtZero: true },
          y: { title: { display: true, text: 'Customers' } }
        }
      }
    });
  }

  goToCustomersPage(){
    this.router.navigate(['/customers']);
  }
}
