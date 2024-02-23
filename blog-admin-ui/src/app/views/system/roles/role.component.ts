import { PermissionGrantComponent } from './permission-grant.component';
import { RolesDetailComponent } from './roles-detail.component';
import { MessageConstants } from './../../../shared/constants/message.constants';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { DialogService, DynamicDialogComponent } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiRoleApiClient, RoleDto, RoleDtoPagedResult } from 'src/app/api/admin-api.service.generated';
import { AlertService } from 'src/app/shared/services/alert.service';

@Component({
  selector: 'app-role',
  templateUrl: './role.component.html'
})
export class RoleComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  public blockedPanel: boolean = false;

  //Paging variables
  public pageIndex: number = 1;
  public pageSize: number = 10;
  public totalCount: number;

  //Business variables
  public items: RoleDto[];
  public selectedItems: RoleDto[] = [];
  public keyword: string = '';

  constructor(private roleService: AdminApiRoleApiClient, public dialogService: DialogService, private alertService: AlertService, private confirmationService: ConfirmationService) { }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.toggleBlockUI(true);
    this.roleService.getRolesPaging(this.keyword, this.pageIndex, this.pageSize).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
      next: (res: RoleDtoPagedResult) => {
        this.items = res.results;
        this.totalCount = res.rowCount;
        this.toggleBlockUI(false);
      },
      error: (err) => {
        this.toggleBlockUI(false);
      }
    })
  }

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
  showPermissionModal(id: string, name: string) {
    const ref = this.dialogService.open(PermissionGrantComponent, {
      data: { id: id },
      header: name,
      width: '70%'
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: RoleDto) => {
      if (data) {
        this.alertService.showSuccess(
          MessageConstants.UPDATED_OK_MSG
        );
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
    const ref = this.dialogService.open(RolesDetailComponent, {
      data: { id: id },
      header: 'Cập nhật quyền',
      width: '70%'
    });
    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;

    ref.onClose.subscribe((data: RoleDto) => {
      if (data) {
        this.alertService.showSuccess(MessageConstants.UPDATED_OK_MSG);
        this.selectedItems = [];
        this.loadData();
      }
    })
  }
  showAddModal() {
    const ref = this.dialogService.open(RolesDetailComponent, {
      header: 'Thêm mới quyền',
      width: '70%'
    });

    const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
    const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
    const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
    dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
    ref.onClose.subscribe((data: RoleDto) => {
      if (data) {
        this.alertService.showSuccess(MessageConstants.CREATED_OK_MSG);
        this.selectedItems = [];
        this.loadData();
      }
    })
  }
  deleteItems() {
    if (this.selectedItems.length == 0) {
      this.alertService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
      return;
    }
    var ids = [];
    this.selectedItems.forEach((item) => {
      ids.push(item.id);
    });

    this.confirmationService.confirm({
      message: MessageConstants.CONFIRM_DELETE_MSG,
      accept: () => {
        this.deleteItemsConfirm(ids);
      }
    })
  }

  deleteItemsConfirm(ids: any[]) {
    this.toggleBlockUI(true);

    this.roleService.deleteRoles(ids).subscribe({
      next: () => {
        this.alertService.showSuccess(
          MessageConstants.DELETED_OK_MSG
        );
        this.loadData();
        this.selectedItems = [];
        this.toggleBlockUI(false);
      },
      error: () => {
        this.toggleBlockUI(false);
      },
    });
  }
}