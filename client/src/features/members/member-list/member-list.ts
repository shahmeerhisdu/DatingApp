import { Component, inject } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { Observable } from 'rxjs';
import { Member } from '../../../types/member';
import { AsyncPipe } from '@angular/common';
import { MembersCard } from "../members-card/members-card";
import { PaginatedResult } from '../../../types/pagination';

@Component({
  selector: 'app-member-list',
  imports: [AsyncPipe, MembersCard], //handle the subscription of an observeable.
  templateUrl: './member-list.html',
  styleUrl: './member-list.css'
})
export class MemberList {
  private memberService = inject(MemberService);
  protected paginatedMembers$ : Observable<PaginatedResult<Member>>; // by convention we need to add the $ sign for the observeable, and if we don't initialize this in the constructor we will get the error, but because we are not going to be making any fetch request from this memberlist component that is happening inside the service, and typically we do fetch operation inside an angular cycle oninit method, but instead of faking this intialization with an empty array we can use that in ihe constructor.

  constructor(){
    this.paginatedMembers$ = this.memberService.getMembers();
  }
}
