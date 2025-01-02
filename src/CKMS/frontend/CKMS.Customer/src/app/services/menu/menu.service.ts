import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpService } from '../http/http.service';
import { Observable } from 'rxjs';
import { KitchenListDTO, MenuItemDTO, MenuItemListDTO } from '../../models/response/menuItem';

@Injectable({
  providedIn: 'root'
})
export class MenuService {

  baseUrl = environment.inventoryServiceDomain;
  adminBaseUrl = environment.adminServiceDomain;

  constructor(private apiService: HttpService) { }

  getAllKitchen():Observable<KitchenListDTO>{
    var apiUrl = `${this.adminBaseUrl}/api/admin/get-all-kitchen?pageSize=10&pageNumber=1`;
    return this.apiService.getData<KitchenListDTO>(apiUrl);
  }
  
  getMenu(kitchenId: string, categoryId: number):Observable<MenuItemListDTO>{
    var apiUrl = `${this.baseUrl}/api/menu/${kitchenId}/get-all-menu`;
    if(categoryId != 0)
      apiUrl += `?categoryId=${categoryId}`;

    return this.apiService.getData<MenuItemListDTO>(apiUrl);
  }

  getMenuItemDetails(menuItemId: number):Observable<MenuItemDTO>{
    var apiUrl = `${this.baseUrl}/api/menu/get-menu-item/${menuItemId}`;

    return this.apiService.getData<MenuItemDTO>(apiUrl);
  }
}
