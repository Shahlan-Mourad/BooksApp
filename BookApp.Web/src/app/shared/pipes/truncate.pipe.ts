import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'truncate',
  standalone: true
})
export class TruncatePipe implements PipeTransform {
  transform(value: string, limit: number = 50, ellipsis: string = '...'): string {
    if (!value) return '';
    value = value.trim();
    if (limit < 1) return ellipsis;
    if (value.length <= limit) return value;

    // Try to cut at the nearest space
    const truncated = value.substring(0, limit);
    const lastSpace = truncated.lastIndexOf(' ');
    return (lastSpace > 0 ? truncated.substring(0, lastSpace) : truncated) + ellipsis;
  }
}