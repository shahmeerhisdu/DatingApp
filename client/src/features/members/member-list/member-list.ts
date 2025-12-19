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
    console.log('Modal submitted data: ', data);
  }

  resetFilters(){
    this.memberParams = new MemberParams();
    this.loadMembers();
  }
}
