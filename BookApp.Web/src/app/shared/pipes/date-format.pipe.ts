import { Pipe, PipeTransform, Inject, LOCALE_ID } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({
  name: 'dateFormat',
  standalone: true
})
export class DateFormatPipe implements PipeTransform {
  private datePipe: DatePipe;

  constructor(@Inject(LOCALE_ID) private locale: string) {
    this.datePipe = new DatePipe(this.locale);
  }

  transform(value: string | Date | null | undefined, format: string = 'mediumDate'): string {
    if (!value) {
      return '';
    }
    return this.datePipe.transform(value, format) ?? '';
  }
}