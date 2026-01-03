import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Member } from '../../types/member';
import { PaginatedResult } from '../../types/pagination';

@Injectable({
  providedIn: 'root'
})
export class LikesService {
  private baseUrl  = environment.apiUrl;
  private http = inject(HttpClient);

  likedIds = signal<string[]>([]);

  toggleLike(targetMemberId: string) {
    return this.http.post(`${this.baseUrl}likes/${targetMemberId}`, {})
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());
    params = params.append('predicate', predicate);

    return this.http.get<PaginatedResult<Member>>(`${this.baseUrl}likes`, { params } );
  }

  getLikedIds() {
    return this.http.get<string[]>(`${this.baseUrl}likes/list`).subscribe({
      next: ids => this.likedIds.set(ids)
    })
  }

  clearLikedIds() {
    this.likedIds.set([]);
  }

}
