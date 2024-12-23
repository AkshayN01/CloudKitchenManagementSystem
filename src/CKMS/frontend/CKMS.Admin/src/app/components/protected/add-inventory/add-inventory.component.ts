import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { InventoryService } from '../../../services/inventory/inventory.service';

@Component({
  selector: 'app-add-inventory',
  templateUrl: './add-inventory.component.html',
  styleUrl: './add-inventory.component.css'
})
export class AddInventoryComponent {
  inventoryForm: FormGroup;
  isSubmitted: boolean = false;
  
  constructor(private fb: FormBuilder, private snackBar: MatSnackBar, private inventoryService: InventoryService) {
    this.inventoryForm = this.fb.group({
      inventoryName: ['', Validators.required],
      quantity: [0, [Validators.required, Validators.min(0)]],
      unit: [0, [Validators.required, Validators.min(1)]],
      restockThreshold: [0, [Validators.required, Validators.min(0)]],
      maxStockLevel: [0, [Validators.required, Validators.min(1)]]
    });
  }
  onSubmit(): void {
    if (this.inventoryForm.valid) {
      const payload = this.inventoryForm.value;
      this.inventoryService.addInventory(this.inventoryForm.value).subscribe((res) => {
        if(res){
          this.snackBar.open('Inventory added successfully!', 'Close', { duration: 3000 });
          this.isSubmitted = true;
          this.inventoryForm.reset({
            inventoryName: '',
            quantity: 0,
            unit: 0,
            restockThreshold: 0,
            maxStockLevel: 0
          });
        }
        else{
          this.snackBar.open('Inventory failed to add. Please try again.', 'Close', { duration: 3000 });
        }
      });
    }
  }

  onAddMore(): void {
    this.isSubmitted = false;
  }
}
