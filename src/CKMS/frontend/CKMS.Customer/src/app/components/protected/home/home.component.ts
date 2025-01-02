import { Component } from '@angular/core';
import { MenuItemDTO } from '../../../models/response/menuItem';
import { MenuService } from '../../../services/menu/menu.service';
import { OrderService } from '../../../services/order/order.service';
import { UtilityService } from '../../../services/utility/utility.service';
import { OrderItemPayload, OrderPayload } from '../../../models/request/order';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  orderId: string = '';
  kitchenId!: string;
  menuItems: MenuItemDTO[] = [];
  cart: { [menuItemId: number]: number } = {}; // To track quantities in cart

  constructor(private menuService: MenuService, private orderService: OrderService, private utilityService: UtilityService, private router: Router) {}

  ngOnInit(): void {
    this.fetchMenuItems();
  }

  fetchMenuItems(): void {
    this.menuService.getAllKitchen().subscribe((res) => {
      this.kitchenId = res.kitchenList[0].kitchenId;
      this.menuService.getMenu(this.kitchenId, 0).subscribe((menuRes) => {
        this.menuItems = menuRes.menuItems;
        this.orderService.getCart().subscribe((cartRes) => {
          if(cartRes != null){
            this.orderId = cartRes.orderId;
            cartRes.items.forEach((item) => {
              this.cart[item.menuItemId] = item.quantity;
            })
          }
        })
      })
    });
  }

  addToCart(menuItem: MenuItemDTO): void {
    if (this.cart[menuItem.menuItemId]) {
      this.cart[menuItem.menuItemId]++;
    } else {
      this.cart[menuItem.menuItemId] = 1;
    }
  }

  checkout(): void {
    const items: OrderItemPayload[] = Object.entries(this.cart).map(
      ([menuItemId, quantity]) => ({
        menuItemId: Number(menuItemId),
        quantity
      })
    );

    const payload: OrderPayload = {
      orderId: this.orderId,
      kitchenId: this.kitchenId,
      items
    };

    this.orderService.addToCart(payload).subscribe((res) => {
      if(res == null)
        this.utilityService.openSnackBar("A different order is already present in the cart. PLease clear or continue with it");
        this.router.navigate(['/cart']);
    });
  }
}
