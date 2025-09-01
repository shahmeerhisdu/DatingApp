import { CanDeactivateFn } from '@angular/router';
import { MemberProfile } from '../../features/members/member-profile/member-profile';

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberProfile> = (component) => {
  //here we have the access to the component from which we are trying to move away from, so inside our component inside our member profile we have a property which we need to change from for the edit form that we can use inside prevent and unsaved changes guard, and when we gonna access to the form we can check to see if its dirty, if its dirty we can really ask the user if they want to move away.
  if(component.editForm?.dirty){
    return confirm('Are you sure you want to continue? All unsaved changes will be lost')
  }

  return true;
};
