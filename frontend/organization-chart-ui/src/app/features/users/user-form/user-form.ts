import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { UserService } from '../../../core/services/user.service';
import { DepartmentService } from '../../../core/services/department.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { Department } from '../../../core/models/department';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatOptionModule],
  templateUrl: './user-form.html',
  styleUrl: './user-form.scss',
})
export class UserForm implements OnInit {
  form: FormGroup;
  departments: Department[] = [];
  userId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private departmentService: DepartmentService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.form = this.fb.group({
      employeeNumber: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      title: [''],
      departmentId: [null, Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadDepartments();

    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.userId = Number(id);
      this.loadUser(this.userId);
    }
  }

  loadDepartments(): void {
    this.departmentService.getAll().subscribe({
      next: (data) => this.departments = data,
      error: (err) => console.error(err)
    });
  }

  loadUser(id: number): void {
    this.userService.getById(id).subscribe({
      next: (user) => {
        this.form.patchValue({
          employeeNumber: user.employeeNumber,
          firstName: user.firstName,
          lastName: user.lastName,
          title: user.title,
          departmentId: user.departmentId
        });
      },
      error: (err) => console.error(err)
    });
  }

  onSubmit(): void {
    if (this.form.invalid)
      return;

    if (this.userId == null) {
      this.userService.create(this.form.value)
        .subscribe(() => this.router.navigate(['/users']));
    } else {
      this.userService.update(this.userId, this.form.value)
        .subscribe(() => this.router.navigate(['/users']));
    }
  }
}
