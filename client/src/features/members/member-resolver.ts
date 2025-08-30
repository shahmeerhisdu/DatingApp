import { ResolveFn, Router } from '@angular/router';
import { MemberService } from '../../core/services/member-service';
import { inject } from '@angular/core';
import { Member } from '../../types/member';
import { EMPTY } from 'rxjs';

export const memberResolver: ResolveFn<Member> = (route, state) => {
  // root resolver is used to get the data before our route is activated. When we go to the root we gonna use this resolver to resolve the data and then its gonna be available inside our router.
  const memberService = inject(MemberService)
  const router = inject(Router);
  const memberId = route.paramMap.get('id');

  if(!memberId){
    router.navigateByUrl('/not-found');
    return EMPTY;
  }
  return memberService.getMember(memberId);
};
