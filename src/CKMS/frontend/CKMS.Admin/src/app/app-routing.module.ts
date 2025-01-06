import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { VerifyAccountComponent } from './components/verify-account/verify-account.component';
import { HeaderComponent } from './components/protected/header/header.component';
import { OrdersComponent } from './components/protected/orders/orders.component';
import { InventoryDetailComponent } from './components/protected/inventory-detail/inventory-detail.component';
import { ViewInventoriesComponent } from './components/protected/view-inventories/view-inventories.component';
import { CustomerSummaryComponent } from './components/protected/customer-summary/customer-summary.component';
import { ReportComponent } from './components/protected/report/report.component';
import { CustomersComponent } from './components/protected/customers/customers.component';
import { OrderDetailComponent } from './components/protected/order-detail/order-detail.component';

const routes: Routes = [
  {path:'', pathMatch: 'full', redirectTo: '/login'},
  {path: 'login', component: LoginComponent},
  {path: 'register', component: RegisterComponent},
  {path: 'verify-account', component: VerifyAccountComponent},
  {path: '', component: HeaderComponent,
  children:[
    {path: 'inventory-detail', component: InventoryDetailComponent},
    {path: 'inventories', component: ViewInventoriesComponent},
    {path: 'orders', component: OrdersComponent},
    {path: 'order-detail/:id', component: OrderDetailComponent},
    {path: 'customer-summary/:id', component: CustomerSummaryComponent},
    {path: 'report', component: ReportComponent},
    {path: 'customers', component: CustomersComponent}
  ],
}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
