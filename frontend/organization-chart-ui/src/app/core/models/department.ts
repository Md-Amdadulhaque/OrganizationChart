export interface Department {
  id: number;
  departmentCode: string;
  name: string;
  parentDepartmentId?: number;
}