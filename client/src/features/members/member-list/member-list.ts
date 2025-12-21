import { Component, inject, OnInit, signal, ViewChild } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { Observable } from 'rxjs';
import { Member, MemberParams } from '../../../types/member';
import { AsyncPipe } from '@angular/common';
import { MembersCard } from "../members-card/members-card";
import { PaginatedResult } from '../../../types/pagination';
import { Paginator } from "../../../shared/paginator/paginator";
import { FilterModal } from '../filter-modal/filter-modal';

@Component({
  selector: 'app-member-list',
  imports: [MembersCard, Paginator, FilterModal], //handle the subscription of an observeable.
  templateUrl: './member-list.html',
  styleUrl: './member-list.css'
})
export class MemberList implements OnInit {
  @ViewChild('filterModal') modal!: FilterModal;
  private memberService = inject(MemberService);
  protected paginatedMembers = signal<PaginatedResult<Member> | null>(null);

  protected memberParams = new MemberParams();
  private updatedParams = new MemberParams();

  
  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers(){
    this.memberService.getMembers(this.memberParams).subscribe({
      next: (result) => this.paginatedMembers.set(result)
    });
  }

  onPageChange(event: { pageNumber: number, pageSize: number }){
    this.memberParams.pageNumber = event.pageNumber;
    this.memberParams.pageSize = event.pageSize;
    this.loadMembers();
  }

  openModal(){
    this.modal.open();
  }

  onClose(){
    console.log('Modal closed');
  }

  onFilterChange(data: MemberParams){
    this.memberParams = {...data}; //If I do = data on both it will reference the same object and changes in one will affect the other. So by using the speread operator we create a shallow copy.
    this.updatedParams = {...data};
    this.loadMembers();
  }

  resetFilters(){
    this.memberParams = new MemberParams();
    this.loadMembers();
  }

  get displayMessage(): string{
    const defaultParams = new MemberParams();

    const filters: string[] = [];

    if(this.updatedParams.gender){
      filters.push(this.updatedParams.gender + 's');
    }else{
      filters.push('Males, Females');
    }

    if(this.updatedParams.minAge !== defaultParams.minAge || this.updatedParams.maxAge !== defaultParams.maxAge){
      filters.push(` ages ${this.updatedParams.minAge} - ${this.updatedParams.maxAge}`);
    }

    filters.push(this.updatedParams.orderBy === 'lastActive' ? ' Recently Active' : ' Newest Members');
    return filters.length > 0 ?  `Selected Filters: ${filters.join('  | ')}` : 'All Members';
  }
}
