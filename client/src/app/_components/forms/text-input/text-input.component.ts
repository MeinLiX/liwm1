import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})
export class TextInputComponent implements ControlValueAccessor {
  @Input() placeholder = '';
  @Input() label = '';
  @Input() type = 'text';

  private readonly notAllowedCharactersForUsername = /[^a-zA-Z0-9]/g;
  private readonly notAllowedCharactersForPassword = /[^\dA-Za-z!@#$%^&*()_+{}\[\]:;<>,.?~\/\\]/g;

  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
  }

  writeValue(obj: any): void {
  }

  registerOnChange(fn: any): void {
  }

  registerOnTouched(fn: any): void {
  }

  validateInput(event: Event) {
    //TODO: Fix button activating for anonymous user if this control text is empty after writing cyryllic text
    const inputElement = event.target as HTMLInputElement;
    const input = inputElement.value;
    let regex: RegExp | null = null;
    if (this.type !== 'password') {
      regex = this.notAllowedCharactersForUsername;
    } else if (this.type === 'password') {
      regex = this.notAllowedCharactersForPassword;
    }

    if (regex) {
      inputElement.value = input.replace(regex, '');
    }
  }

  get control(): FormControl {
    return this.ngControl.control as FormControl;
  }
}