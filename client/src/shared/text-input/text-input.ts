import { Component, input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  imports: [ReactiveFormsModule],
  templateUrl: './text-input.html',
  styleUrl: './text-input.css'
})
export class TextInput implements ControlValueAccessor {
  label = input<string>('');
  type =  input<string>('text');
  maxDate = input<string>('');

  /**
   *
   */
  constructor(@Self() public ngControl: NgControl) { 
    //@Self() decorator:  this is also referred to as the dependency injection modifier and it is gonna tell the angular that only look for the dependency that we are injecting on the current element and not to serch up the ijector hierarchy, in the tree so its parents or its ancestors. As we want this ngControl to be very specific to the instance of the text input that is being used inside our form. As if we don't include this and when we use this text input component and becuase the angular tries to reuse things that have been injected then it is gonna look up the component tree or injected tree to search for any instance of this ngControl.
    
    //and if don't have this (@Self()) then it could use the control from another text input (because its a reuseable component and will not be differenciated) and that would give us that us quite tricky to debug so we have to use the Self(), as this gurantees that this control is gonna be unique for the text input that we are using inside our form, and because our form will have the several inputs then each one of these control instances will be going to be unique.

    this.ngControl.valueAccessor = this; // we are saying that our text input is the type of ngControl and we are assigning the textinput to the ngControl value accessor
  }

  writeValue(obj: any): void {
  }
  registerOnChange(fn: any): void {
  }
  registerOnTouched(fn: any): void {
  }

  get control(): FormControl{
    return this.ngControl.control as FormControl
  }


}
