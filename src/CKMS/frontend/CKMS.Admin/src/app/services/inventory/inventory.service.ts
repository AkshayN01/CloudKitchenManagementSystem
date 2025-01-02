import { Injectable } from '@angular/core';
import { HttpService } from '../http/http.service';
import { environment } from '../../../environments/environment';
import { InventoryPayload, InventoryUpdatePayload } from '../../models/request/inventory/inventory';
import { Observable } from 'rxjs';
import { InventoryDTO, InventoryListDTO } from '../../models/response/inventory/inventory';
import { InventoryMovementPayload, InventoryMovementUpdatePayload } from '../../models/request/inventory/inventoryMovement';
import { InventoryMovementDTO, InventoryMovementListDTO } from '../../models/response/inventory/inventoryMovement';

@Injectable({
  providedIn: 'root'
})
export class InventoryService {

  baseUrl = environment.inventoryServiceDomain;

  constructor(private apiService: HttpService) { }

  addInventory(payload: InventoryPayload): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/inventory/add-inventory`;
    return this.apiService.postData<boolean, InventoryPayload>(apiUrl, payload);
  }

  updateInventory(payload: InventoryUpdatePayload): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/inventory/update-inventory`;
    return this.apiService.postData<boolean, InventoryUpdatePayload>(apiUrl, payload);
  }

  deleteInventory(inventoryId: number): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/inventory/delete-inventory/${inventoryId}`;
    return this.apiService.deleteData<boolean>(apiUrl);
  }

  getInventory(inventoryId: number): Observable<InventoryDTO>{
    var apiUrl = `${this.baseUrl}/api/inventory/get-inventory/${inventoryId}`;
    return this.apiService.getData<InventoryDTO>(apiUrl);
  }

  getAllInventory(pageSize: number, pageNumber: number): Observable<InventoryListDTO>{
    var apiUrl = `${this.baseUrl}/api/inventory/get-all-inventory?pageSize=${pageSize}&pageNumber=${pageNumber}`;
    return this.apiService.getData<InventoryListDTO>(apiUrl);
  }

  //inventory movement
  addInventoryMovement(payload: InventoryMovementPayload): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/inventory/add-inventory-movement`;
    return this.apiService.postData<boolean, InventoryMovementPayload>(apiUrl, payload);
  }

  updateInventoryMovement(payload: InventoryMovementUpdatePayload): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/inventory/update-inventory-movement`;
    return this.apiService.postData<boolean, InventoryMovementUpdatePayload>(apiUrl, payload);
  }

  deleteInventoryMovement(inventoryMovementId: number): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/inventory/delete-inventory-movement/${inventoryMovementId}`;
    return this.apiService.deleteData<boolean>(apiUrl);
  }

  getInventoryMovement(inventoryId: number): Observable<InventoryMovementDTO>{
    var apiUrl = `${this.baseUrl}/api/inventory/get-inventory-movement/${inventoryId}`;
    return this.apiService.getData<InventoryMovementDTO>(apiUrl);
  }

  getAllInventoryMovement(inventoryId: number, pageSize: number, pageNumber: number): Observable<InventoryMovementListDTO>{
    var apiUrl = `${this.baseUrl}/api/inventory/get-all-inventory-movement/${inventoryId}?pageSize=${pageSize}&pageNumber=${pageNumber}`;
    return this.apiService.getData<InventoryMovementListDTO>(apiUrl);
  }
}
