import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';

import { environment } from '../environments/environment';

import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { JWT_OPTIONS, JwtModule } from '@auth0/angular-jwt';

import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatMenuModule } from '@angular/material/menu';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatStepperModule } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatRippleModule } from '@angular/material/core';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatDividerModule } from '@angular/material/divider';
import { MatRadioModule } from '@angular/material/radio';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatListModule } from '@angular/material/list';

import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { VerifyAccountComponent } from './components/verify-account/verify-account.component';
import { OrdersComponent } from './components/protected/orders/orders.component';
import { HeaderComponent } from './components/protected/header/header.component';
import { ConfirmationDialogComponent } from './components/confirmation-dialog/confirmation-dialog.component';
import { OrderDetailComponent } from './components/protected/order-detail/order-detail.component';
import { AddInventoryComponent } from './components/protected/add-inventory/add-inventory.component';
import { ViewInventoriesComponent } from './components/protected/view-inventories/view-inventories.component';
import { InventoryDetailComponent } from './components/protected/inventory-detail/inventory-detail.component';
import { SessionService } from './services/session/session.service';
import { AuthInterceptor } from './services/http/auth';
import { CustomerSummaryComponent } from './components/protected/customer-summary/customer-summary.component';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';
import { ReportComponent } from './components/protected/report/report.component';
import { CustomersComponent } from './components/protected/customers/customers.component';

export function tokenGetter() {
  return localStorage.getItem(environment.authStorageName);
}

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    VerifyAccountComponent,
    OrdersComponent,
    HeaderComponent,
    ConfirmationDialogComponent,
    OrderDetailComponent,
    AddInventoryComponent,
    ViewInventoriesComponent,
    InventoryDetailComponent,
    CustomerSummaryComponent,
    ReportComponent,
    CustomersComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    MatIconModule,
    MatCardModule,
    MatMenuModule,
    MatButtonModule,
    MatToolbarModule,
    MatInputModule,
    MatFormFieldModule,
    MatStepperModule,
    MatChipsModule,
    MatSelectModule,
    MatPaginatorModule,
    MatRippleModule,
    MatGridListModule,
    MatDividerModule,
    MatRadioModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule,
    MatTableModule,
    MatListModule,
    JwtModule.forRoot({
      jwtOptionsProvider: {
        provide: JWT_OPTIONS,
        useFactory: () => ({
          tokenGetter: tokenGetter,
          allowedDomains: [
            environment.adminServiceDomain, 
            environment.orderServiceDomain, 
            environment.inventoryServiceDomain, 
            environment.notificationServiceDomain
          ],
          disallowedRoutes: 
          [
            environment.loginAPIUrl, 
            environment.registerAPIUrl
          ],
        }),
        deps: []
      }
    })
  ],
  providers: [
    provideAnimationsAsync(),
    SessionService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    provideCharts(withDefaultRegisterables())
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
