import { Component, OnInit } from '@angular/core';
import { InventoryDTO, InventoryListDTO } from '../../../models/response/inventory/inventory';
import { InventoryService } from '../../../services/inventory/inventory.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-view-inventories',
  templateUrl: './view-inventories.component.html',
  styleUrl: './view-inventories.component.css'
})
export class ViewInventoriesComponent implements OnInit{
  inventories: InventoryDTO[] = [];
  totalCount: number = 0;
  pageIndex: number = 1;
  pageSize: number = 10;

  constructor(private inventoryService: InventoryService, private router: Router) {}

  ngOnInit(): void {
    this.fetchInventories();
  }

  fetchInventories(): void {
    this.inventoryService.getAllInventory(this.pageSize, this.pageIndex).subscribe((res) => {
      this.inventories = res.inventories;
      this.totalCount = res.totalCount;
    });
  }

  onPageChange(event: any): void {
    this.pageIndex = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.fetchInventories();
  }

  viewInventory(inventory: InventoryDTO): void {
    this.router.navigate(['/inventory-detail'], { state: { inventory } });
  }
}
