import { Component, inject, input, OnInit, output, signal, Signal } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { RegisterCreds, User } from '../../../types/user';
import { AccountService } from '../../../core/services/account-service';
import { JsonPipe } from '@angular/common';
import { TextInput } from "../../../shared/text-input/text-input";
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, TextInput],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {

  private accountService = inject(AccountService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  // membersFromHome = input.required<User[]>(); // this is the other way of getting the data from the parent.
  cancelRegister = output<boolean>();
  protected creds = {} as RegisterCreds
  protected credentialsForm: FormGroup;
  protected profileForm: FormGroup;
  protected currentStep =  signal(1);
  protected validationErrors = signal<string[]>([]); // to display the errors that we are not capturing on the client side but we are getting those from the API side, so we can show them on the form.

  constructor() {
    this.credentialsForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      displayName: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    });

    this.profileForm = this.fb.group({
      gender: ['male', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
    })

    //custom validation only occurs on the confirm password if we correct the password after confirm password we will get invalid because the confirm password does not know that the password field has been changed so we need some mechanism for that as well that is done in here.

    this.credentialsForm.controls['password'].valueChanges.subscribe(() => {
      this.credentialsForm.controls['confirmPassword'].updateValueAndValidity();
    })

  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const parent = control.parent;

      if (!parent) return null;

      const matchValue = parent.get(matchTo)?.value;

      return control.value === matchValue ? null : { passwordMismatch: true }
    }
  }

  nextStep(){
    if(this.credentialsForm.valid){
      this.currentStep.update(prevStep => prevStep + 1);
    }
  }

  prevStep(){
    this.currentStep.update(prevStep => prevStep - 1);
  }

  getMaxDate(){
    const today = new Date();
    today.setFullYear(today.getFullYear() - 18);
    return today.toISOString().split('T')[0];
  }

  register() {

    if(this.profileForm.valid && this.credentialsForm.valid){
      const formData = {... this.credentialsForm.value, ...this.profileForm.value};
      this.accountService.register(formData).subscribe({
        next: () => {
          this.router.navigateByUrl('/members');
        },
        error: error => {
          console.log(error);
          this.validationErrors.set(error);
        }
      })
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
