import { Component } from '@angular/core';
import { OrderCartDTO } from '../../../models/response/order';
import { AddressDTO } from '../../../models/response/customer';
import { OrderService } from '../../../services/order/order.service';
import { UtilityService } from '../../../services/utility/utility.service';
import { ProfileService } from '../../../services/user-profile/profile.service';
import { AddressService } from '../../../services/customer-address/address.service';
import { ConfirmOrderPayload, DiscountUsagePayload } from '../../../models/request/order';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent {
  orderCart: OrderCartDTO | null = null;
  addresses: AddressDTO[] = [];
  selectedAddressId: string | null = null;
  couponCode: string = '';
  isCouponApplied: boolean = false;
  orderPlaced: boolean = false;

  constructor(private orderService: OrderService, private addressService: AddressService, private utilityService: UtilityService) {}

  ngOnInit(): void {
    this.fetchCartSummary();
    this.fetchAddresses();
  }

  fetchCartSummary(): void {
    this.orderService.getCart().subscribe((res) => {
      this.orderCart = res;
    });
  }

  fetchAddresses(): void {
    this.addressService.getAllAddress().subscribe((res) => {
      this.addresses = res;
      if (this.addresses.length > 0) {
        this.selectedAddressId = this.addresses[0].addressId; // Default to the first address
      };
    });
  }

  applyCoupon(): void {
    if (!this.orderCart) {
      this.utilityService.openSnackBar('Unable to apply coupon. Please try again.');
      return;
    }

    const payload: DiscountUsagePayload = {
      couponCode: this.couponCode,
      orderId: this.orderCart.orderId,
      appliedDate: new Date().toISOString()
    };

    this.orderService.applyDiscount(payload).subscribe((res) => {
      if(res){
        this.isCouponApplied = true;
        this.utilityService.openSnackBar('Coupon applied successfully!');
      }
      else{
        this.utilityService.openSnackBar('Error');
      }
    });
  }

  cancelCoupon(): void {
    if (!this.orderCart) {
      this.utilityService.openSnackBar('Unable to cancel coupon. Please try again.');
      return;
    }

    const payload: DiscountUsagePayload = {
      couponCode: this.couponCode,
      orderId: this.orderCart.orderId,
      appliedDate: new Date().toISOString()
    };

    this.orderService.cancelDiscount(payload).subscribe((res) => {
      if(res){
        this.isCouponApplied = false;
        this.couponCode = '';
        this.utilityService.openSnackBar('Coupon canceled successfully!');
      }
    });
  }

  confirmOrder(): void {
    if (!this.orderCart || !this.selectedAddressId) {
      this.utilityService.openSnackBar('Please select an address and try again.');
      return;
    }

    const payload: ConfirmOrderPayload = {
      orderId: this.orderCart.orderId,
      paymentMethod: 1, // Cash on Delivery
      addressId: this.selectedAddressId
    };

    this.orderService.confirmOrder(payload).subscribe((res) => {
      this.orderPlaced = res;
      if(res)
        this.utilityService.openSnackBar('Order confirmed successfully!');
      else
        this.utilityService.openSnackBar('Error confirming order');
    });
  }
}
