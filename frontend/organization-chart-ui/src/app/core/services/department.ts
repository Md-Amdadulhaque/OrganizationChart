import { Injectable } from "@angular/core";
import { environment } from "../../../environment/environment";
import { HttpClient } from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class Department {

  private api = environment.apiUrl + '/departments';

  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<Department[]>(this.api);
  }

  getById(id: number) {
    return this.http.get<Department>(`${this.api}/${id}`);
  }

  create(dto: any) {
    return this.http.post(this.api, dto);
  }

  update(id: number, dto: any) {
    return this.http.put(`${this.api}/${id}`, dto);
  }

  delete(id: number) {
    return this.http.delete(`${this.api}/${id}`);
  }
}