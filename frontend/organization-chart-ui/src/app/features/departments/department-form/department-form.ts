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
import { ActivatedRoute } from '@angular/router';
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
  departmentId: number | null = null;
  constructor(
    private fb: FormBuilder,
    private departmentService: DepartmentService,
    private router: Router,
    private route: ActivatedRoute
  ) {

    this.form = this.fb.group({
      departmentCode: ['', Validators.required],
      name: ['', Validators.required],
      parentDepartmentId: [null]
    });

  }
  ngOnInit(): void {

    this.loadDepartments();

    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.departmentId = Number(id);
      this.loadDepartment(this.departmentId);
    }
  }
  loadDepartments(): void {
    this.departmentService.getAll().subscribe({
      next: (data) => this.departments = data,
      error: (err) => console.error(err)
    });
  }
  loadDepartment(id: number): void {

    this.departmentService.getById(id).subscribe({

      next: (department) => {

        this.form.patchValue({
          departmentCode: department.departmentCode,
          name: department.name,
          parentDepartmentId: department.parentDepartmentId
        });

      },

      error: (err) => console.error(err)

    });

  }
  onSubmit(): void {

    if (this.form.invalid)
        return;

    if (this.departmentId == null) {

        this.departmentService.create(this.form.value)
            .subscribe(() => this.router.navigate(['/departments']));

    } else {

        this.departmentService.update(this.departmentId, this.form.value)
            .subscribe(() => this.router.navigate(['/departments']));

    }

}


}
