import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { ImportService } from '../../core/services/import.service';

@Component({
  selector: 'app-import',
  standalone: true,
  imports: [MatButtonModule],
  templateUrl: './import.html',
  styleUrl: './import.scss',
})
export class Import {
  departmentFile: File | null = null;
  userFile: File | null = null;

  constructor(private importService: ImportService) {}

  onDepartmentSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files?.length) {
      this.departmentFile = input.files[0];
    }
  }

  onUserSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files?.length) {
      this.userFile = input.files[0];
    }
  }

  importDepartments(): void {
    if (!this.departmentFile)
      return;

    this.importService
      .importDepartments(this.departmentFile)
      .subscribe({
        next: () => alert('Departments imported.'),
        error: err => console.error(err)
      });
  }

  importUsers(): void {
    if (!this.userFile)
      return;

    this.importService
      .importUsers(this.userFile)
      .subscribe({
        next: () => alert('Users imported.'),
        error: err => console.error(err)
      });
  }
}
