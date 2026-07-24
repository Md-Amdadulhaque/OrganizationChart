import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { DepartmentService } from '../../../core/services/department.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Department } from '../../../core/models/department';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
@Component({
  selector: 'app-department-form',
  imports: [ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  MatSelectModule, // Usually required if you are using <mat-select>
    MatOptionModule],
  templateUrl: './department-form.html',
  styleUrl: './department-form.scss',
})
export class DepartmentForm {
  form: FormGroup;
  departments: Department[] = [];
  constructor(
    private fb: FormBuilder,
    private departmentService: DepartmentService,
    private router: Router
  ) {

    this.form = this.fb.group({
      departmentCode: ['', Validators.required],
    name: ['', Validators.required],
      parentDepartmentId: [null]
    });

  } 
  ngOnInit(): void {
  this.departmentService.getAll().subscribe({
    next: (data) => {
      this.departments = data;
    },
    error: (err) => {
      console.error(err);
    }
  });
}
  OnSubmit(): void {

    if (this.form.invalid) {
      return;
    }

    this.departmentService.create(this.form.value).subscribe({

      next: () => {
        this.router.navigate(['/departments']);

      },

      error: (err) => {

        console.error(err);

        alert('Failed to create department.');

      }

    });

  }


}
