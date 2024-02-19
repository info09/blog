import { Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';
@Injectable()
export class AlertService{
    constructor(private messageService: MessageService){}

    showSuccess(){
        this.messageService.add({severity: 'success', summary: 'Thành công', detail: 'message'});
    }

    showError(){
        this.messageService.add({severity: 'error', summary: 'Lỗi', detail: 'message'});
    }
}