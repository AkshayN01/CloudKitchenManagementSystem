import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class UtilityService {

  constructor(private matSnackBar: MatSnackBar) { }

  openSnackBar(message: string) {
    this.matSnackBar.open(message, 'Dismiss', { duration: 3000 });
  }
}
