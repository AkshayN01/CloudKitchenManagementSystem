import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { VerifyAccountComponent } from './components/verify-account/verify-account.component';
import { HeaderComponent } from './components/protected/header/header.component';
import { CartComponent } from './components/protected/cart/cart.component';
import { HomeComponent } from './components/protected/home/home.component';
import { OrdersComponent } from './components/protected/orders/orders.component';
import { OrderDetailsComponent } from './components/protected/order-details/order-details.component';

const routes: Routes = [
  {path:'', pathMatch: 'full', redirectTo: '/login'},
  {path: 'login', component: LoginComponent},
  {path: 'register', component: RegisterComponent},
  {path: 'verify-account', component: VerifyAccountComponent},
  {path: '', component: HeaderComponent,
    children:[
      {path: 'cart', component: CartComponent},
      {path: 'home', component: HomeComponent},
      {path: 'orders', component: OrdersComponent},
      {path: 'order-detail/:id', component: OrderDetailsComponent},
    ],
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
