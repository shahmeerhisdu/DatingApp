import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { filter, Observable } from 'rxjs';
import { Member } from '../../../types/member';
import { AgePipe } from '../../../core/pipes/age-pipe';
import { AccountService } from '../../../core/services/account-service';

@Component({
  selector: 'app-member-detailed',
  imports: [RouterLink, RouterLinkActive, RouterOutlet, AgePipe],
  //routeroutlet for these routes that can be navigated to from our member detail component.
  templateUrl: './member-detailed.html',
  styleUrl: './member-detailed.css'
})
export class MemberDetailed implements OnInit {
  protected memberService = inject(MemberService);
  private route = inject(ActivatedRoute);
  private accountService = inject(AccountService);
  private router = inject(Router); // we need to get the hold of the router to get the tile functionality done.
  // protected member$?: Observable<Member>;
  // protected member = signal<Member | undefined>(undefined);
  protected title = signal<string | undefined>('Profile');

  //edit working
  // We need a property inside here to check if this is the current user, we will take a look on to another type of singal to do this, and that is the computed signal and a computed signal use another signal to work out what its value should be.
  protected isCurrentUser = computed(()=>{
    return this.accountService.currentUser()?.id === this.route.snapshot.paramMap.get('id');
  })

  ngOnInit(): void {
    // this.member$ = this.loadMember();
    // this.route.data.subscribe({
    //   next: data => this.member.set(data['member']) //get the member from the root by resolver

    // })
    
    // this.member()!.dateOfBirth = '2025-07-20'
    // console.log(this.member()?.dateOfBirth);
    this.title.set(this.route.firstChild?.snapshot?.title) //getting the title from the route

    // now we have to set the title on the route change, to do that we need to effectively listen to the router events now this.router.events. is an observable and we need to subscribe to it but we don't want all of the events from this so first of all we will need to filter the events we are interested in as we are only interested in the navigation end because the root is the tree of different elements and we only want the end element so we are using the pipe method inside here

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd) // this will effectively be the photos, messages etc.
    ).subscribe({
      next: () =>{
        this.title.set(this.route.firstChild?.snapshot?.title)
      }
    })
  }

  // loadMember() {
  //   const id = this.route.snapshot.paramMap.get('id');
  //   if (!id) return;

  //   return this.memberService.getMember(id);
  // }
}
