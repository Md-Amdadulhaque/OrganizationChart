export interface User {
  id: number;
  employeeNumber: string;
  firstName: string;
  lastName: string;
  title: string;
  departmentId: number;
  departmentName?: string;
}