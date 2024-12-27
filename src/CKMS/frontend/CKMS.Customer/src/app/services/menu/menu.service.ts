import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpService } from '../http/http.service';
import { Observable } from 'rxjs';
import { MenuItemDTO, MenuItemListDTO } from '../../models/response/menuItem';

@Injectable({
  providedIn: 'root'
})
export class MenuService {

  baseUrl = environment.inventoryServiceDomain;

  constructor(private apiService: HttpService) { }

  getMenu(kitchenId: string, pageNumber: number, pageSize: number, categoryId: number):Observable<MenuItemListDTO>{
    var apiUrl = `${this.baseUrl}/api/menu/${kitchenId}/get-all-menu?pageSize=${pageSize}&pageNumber=${pageNumber}`;
    if(categoryId != 0)
      apiUrl += `&categoryId=${categoryId}`;

    return this.apiService.getData<MenuItemListDTO>(apiUrl);
  }

  getMenuItemDetails(menuItemId: number):Observable<MenuItemDTO>{
    var apiUrl = `${this.baseUrl}/api/menu/get-menu-item/${menuItemId}`;

    return this.apiService.getData<MenuItemDTO>(apiUrl);
  }
}
