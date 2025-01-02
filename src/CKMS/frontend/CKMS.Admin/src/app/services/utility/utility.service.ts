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

  convertDate(dateString: string): string {

    // Create a new Date object
    const date = new Date(dateString);

    // Get the day, month, and year components
    const day = ("0" + date.getDate()).slice(-2); // Zero padding
    const month = ("0" + (date.getMonth() + 1)).slice(-2); // Months are zero-based
    const year = date.getFullYear();

    // Format the date in the desired format
    const formattedDate = `${day}/${month}/${year} 00:00:00`;

    console.log(formattedDate); // Output: "01/04/2024 00:00:00"

    return formattedDate;
  }
}
