import { UserDetailComponent } from './user-detail.component';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { DialogService, DynamicDialogComponent } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiUserApiClient, UserDto, UserDtoPagedResult } from 'src/app/api/admin-api.service.generated';
import { MessageConstants } from 'src/app/shared/constants/message.constants';
import { AlertService } from 'src/app/shared/services/alert.service';
import { ChangeEmailComponent } from './change-email.component';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html'
})
export class UserComponent implements OnInit, OnDestroy {
  //System variables
  private ngUnsubscribe = new Subject<void>();
  public blockedPanel: boolean = false;

  //Paging variables
  public pageIndex: number = 1;
  public pageSize: number = 10;
  public totalCount: number;

  //Business variables
  public items: UserDto[];
  public selectedItems: UserDto[] = [];
  public keyword: string = '';

  constructor(private userService: AdminApiUserApiClient,
    public dialogService: DialogService,
    private alertService: AlertService,
    private confirmationService: ConfirmationService) { }
  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
  ngOnInit(): void {
    this.loadData();
  }

  loadData(selectionId = null) {
    this.toggleBlockUI(true);
    this.userService.getAllUserPaging(this.keyword, this.pageIndex, this.pageSize).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
      next: (res: UserDtoPagedResult) => {
        this.items = res.results;
        this.totalCount = res.rowCount;
        if (selectionId != null && this.items.length > 0) {
          this.selectedItems = this.items.filter((x) => x.id == selectionId);
        }
        this.toggleBlockUI(false);
      },
      error: (err) => {
        console.log(err);
        this.toggleBlockUI(false);
      }
    })
  }

  showAddModal() {
    const ref = this.dialogService.open(UserDetailComponent, {
      header: 'Thêm mới người dùng',
      width: '70%'
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: UserDto) => {
      if (data) {
        this.alertService.showSuccess(MessageConstants.CREATED_OK_MSG);
        this.selectedItems = [];
        this.loadData();
      }
    })
  }
  showEditModal() {
    if (this.selectedItems.length == 0) {
      this.alertService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
      return;
    }
    var id = this.selectedItems[0].id;
    const ref = this.dialogService.open(UserDetailComponent, {
      data: { id: id },
      header: 'Cập nhật người dùng',
      width: '70%'
    });

    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: UserDto) => {
      if (data) {
        this.alertService.showSuccess(MessageConstants.UPDATED_OK_MSG);
        this.selectedItems = [];
        this.loadData(data.id);
      }
    })
  }
  deleteItems() {
    if (this.selectedItems.length == 0) {
      this.alertService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
      return;
    }
    var ids = [];
    this.selectedItems.forEach(el => ids.push(el.id));
    this.confirmationService.confirm({
      message: MessageConstants.CONFIRM_DELETE_MSG,
      accept: () => { this.deleteItemsConfirm(ids) }
    })
  }
  deleteItemsConfirm(ids: any[]) {
    this.toggleBlockUI(true);
    this.userService.deleteUsers(ids).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
      next: () => {
        this.alertService.showSuccess(MessageConstants.DELETED_OK_MSG);
        this.selectedItems = [];
        this.loadData();
        this.toggleBlockUI(false);
      },
      error: () => {
        // this.alertService.showError(MessageConstants.DELETED_OK_MSG)
        this.toggleBlockUI(false);
      }
    })
  }
  setPassword(id: string) { }

  changeEmail(id: string) {
    const ref = this.dialogService.open(ChangeEmailComponent,{
      data:{id:id},
      header: 'Đặt lại email',
      width: '70%'
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((res: boolean) => {
      if(res){
        this.alertService.showSuccess(MessageConstants.CHANGE_EMAIL_SUCCCESS_MSG);
        this.selectedItems = [];
        this.loadData();
      }
    })
   }

  assignRole(id: string) { }


  pageChanged(event: any): void {
    this.pageIndex = event.page;
    this.pageSize = event.rows;
    this.loadData();
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.blockedPanel = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
      }, 1000);
    }
  }
}