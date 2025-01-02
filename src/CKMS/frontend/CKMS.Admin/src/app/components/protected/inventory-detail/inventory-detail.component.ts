import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { InventoryService } from '../../../services/inventory/inventory.service';
import { InventoryUpdatePayload } from '../../../models/request/inventory/inventory';
import { InventoryMovementPayload, InventoryMovementUpdatePayload } from '../../../models/request/inventory/inventoryMovement';

@Component({
  selector: 'app-inventory-detail',
  templateUrl: './inventory-detail.component.html',
  styleUrl: './inventory-detail.component.css'
})
export class InventoryDetailComponent {
  inventory: any;
  inventoryForm: FormGroup;
  movements: any[] = [];
  movementForm: FormGroup;
  pageIndex: number = 1;
  pageSize: number = 10;
  totalMovements: number = 0;

  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private inventoryService: InventoryService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    const state = this.router.getCurrentNavigation()?.extras.state;
    this.inventory = state ? state['inventory'] : null;

    this.inventoryForm = this.fb.group({
      inventoryName: [this.inventory?.inventoryName || '', Validators.required],
      quantity: [this.inventory?.quantity || 0, [Validators.required, Validators.min(0)]],
      unit: [this.inventory?.unit || 0, [Validators.required, Validators.min(1)]],
      restockThreshold: [this.inventory?.restockThreshold || 0, [Validators.required, Validators.min(0)]],
      maxStockLevel: [this.inventory?.maxStockLevel || 0, [Validators.required, Validators.min(1)]]
    });

    this.movementForm = this.fb.group({
      movementType: [null, Validators.required],
      quantity: [null, [Validators.required, Validators.min(1)]],
      movementDate: [null, Validators.required]
    });
  }

  ngOnInit(): void {
    this.fetchMovements();
  }

  fetchMovements(): void {
    this.inventoryService.getAllInventoryMovement(this.inventory.inventoryId, this.pageSize, this.pageIndex).subscribe((res) => {
      this.movements = res.inventoryMovements;
      this.totalMovements = res.totalCount;
    });
  }

  updateInventory(): void {
    const payload: InventoryUpdatePayload = { inventoryId: this.inventory.inventoryId, ...this.inventoryForm.value };
    this.inventoryService.updateInventory(payload).subscribe((res) => {
      if(res){
        this.snackBar.open('Inventory updated successfully!', 'Close', { duration: 3000 });
      }
      else{
        this.snackBar.open('Failed to update inventory.', 'Close', { duration: 3000 });
      }
    });
  };

  deleteInventory(): void {
    this.inventoryService.deleteInventory(this.inventory.inventoryId).subscribe((res) => {
      if(res){
        this.snackBar.open('Inventory deleted successfully!', 'Close', { duration: 3000 });
        this.router.navigate(['/inventory-list']);
      }
      else{
        this.snackBar.open('Failed to delete inventory.', 'Close', { duration: 3000 });
      }
    });
  }

  addMovement(): void {
    if (this.movementForm.valid) {
      const payload: InventoryMovementPayload = { inventoryId: this.inventory.inventoryId, ...this.movementForm.value };
      this.inventoryService.addInventoryMovement(payload).subscribe((res) => {
        if(res){
          this.snackBar.open('Movement added successfully!', 'Close', { duration: 3000 });
          this.movementForm.reset();
          this.fetchMovements();
        }
        else{
          this.snackBar.open('Failed to add movement.', 'Close', { duration: 3000 });
        }
      });
    }
  }

  updateMovement(movement: any): void {
    const payload: InventoryMovementUpdatePayload = { ...movement };
    this.inventoryService.updateInventoryMovement(payload).subscribe((res) => {
      if(res){
        this.snackBar.open('Movement updated successfully!', 'Close', { duration: 3000 });
        this.fetchMovements();
      }
      else{
        this.snackBar.open('Failed to update movement.', 'Close', { duration: 3000 });
      }
    });
  }

  deleteMovement(movementId: number): void {
    this.inventoryService.deleteInventoryMovement(movementId).subscribe((res) => {
      if(res){
        this.snackBar.open('Movement deleted successfully!', 'Close', { duration: 3000 });
        this.fetchMovements();
      }
      else{
        this.snackBar.open('Failed to delete movement.', 'Close', { duration: 3000 });
      }
    });
  }

  onPageChange(event: any): void {
    this.pageIndex = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.fetchMovements();
  }
}
