import { Component, inject, OnDestroy, OnInit, signal, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EditableMember, Member } from '../../../types/member';
import { DatePipe } from '@angular/common';
import { MemberService } from '../../../core/services/member-service';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastService } from '../../../core/services/toast-service';

@Component({
  selector: 'app-member-profile',
  imports: [DatePipe, FormsModule],
  templateUrl: './member-profile.html',
  styleUrl: './member-profile.css'
})
export class MemberProfile implements OnInit, OnDestroy {

  @ViewChild('editForm') editForm?: NgForm

  protected memberService = inject(MemberService)
  private toast = inject(ToastService);
  private route = inject(ActivatedRoute);
  protected member = signal<Member | undefined>(undefined);
  protected editableMember: EditableMember = {
    displayName: '',
    description: '',
    city: '',
    country: '',
  }

  ngOnInit(): void {
    this.route.parent?.data.subscribe(data => {
      this.member.set(data['member']);
    })

    this.editableMember = {
      displayName: this.member()?.displayName || '',
      description: this.member()?.description || '',
      city: this.member()?.city || '',
      country: this.member()?.country || '',
    }
  }

  updateProfile() {
    if (!this.member()) return;

    const updatedMember = { ...this.member(), ...this.editableMember } //the second spread operator will override the properties of the member with the updated properties inside the editable member

    console.log('updated member: ', updatedMember)
    this.toast.success('Profile Updated Successfully');
    this.memberService.editMode.set(false);
  }

  ngOnDestroy(): void {
    if (this.memberService.editMode()) {
      this.memberService.editMode.set(false);
    }
  }

}
