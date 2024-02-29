import { Component, OnDestroy, OnInit } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { DialogService, DynamicDialogComponent } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiPostCategoryApiClient, PostCategoryDto, PostCategoryDtoPagedResult } from 'src/app/api/admin-api.service.generated';
import { MessageConstants } from 'src/app/shared/constants/message.constants';
import { AlertService } from 'src/app/shared/services/alert.service';
import { PostCategoryDetailComponent } from './post-category-detail.component';

@Component({
    selector: 'app-post-category',
    templateUrl: './post-category.component.html'
})
export class PostCategoryComponent implements OnInit, OnDestroy {
    private ngUnsubscribe = new Subject<void>();
    public blockedPanel: boolean = false;

    //Paging variables
    public pageIndex: number = 1;
    public pageSize: number = 10;
    public totalCount: number;

    //Business variables
    public items: PostCategoryDto[];
    public selectedItems: PostCategoryDto[] = [];
    public keyword: string = '';

    constructor(private postCategoryService: AdminApiPostCategoryApiClient, public dialogService: DialogService, private alertService: AlertService, private confirmationService: ConfirmationService) { }
    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }
    ngOnInit(): void {
        this.loadData();
    }

    loadData() {
        this.toggleBlockUI(true);
        this.postCategoryService.getPostCategoriesPaging(this.keyword, this.pageIndex, this.pageSize).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
            next: (res: PostCategoryDtoPagedResult) => {
                this.items = res.results;
                this.totalCount = res.rowCount;
                this.toggleBlockUI(false);
            },
            error: () => {
                this.toggleBlockUI(false);
            }
        })
    }

    showAddModal() {
        const ref = this.dialogService.open(PostCategoryDetailComponent, {
            header: 'Thêm mới danh mục',
            width: '70%'
        });
        // const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
        // const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
        // const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
        // dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
        ref.onClose.subscribe((data: PostCategoryDto) => {
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
        const ref = this.dialogService.open(PostCategoryDetailComponent, {
            data: { id: id },
            header: 'Cập nhật danh mục',
            width: '70%'
        });
        // const dialogRef = this.dialogService.dialogComponentRefMap.get(ref);
        // const dynamicComponent = dialogRef?.instance as DynamicDialogComponent;
        // const ariaLabelledBy = dynamicComponent.getAriaLabelledBy();
        // dynamicComponent.getAriaLabelledBy = () => ariaLabelledBy;
        ref.onClose.subscribe((data: PostCategoryDto) => {
            if (data) {
                this.alertService.showSuccess(MessageConstants.UPDATED_OK_MSG);
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
        this.selectedItems.forEach(el => ids.push(el.id));
        this.confirmationService.confirm({
            message: MessageConstants.CONFIRM_DELETE_MSG,
            accept: () => this.deleteItemsConfirm(ids)
        })
    }
    deleteItemsConfirm(ids: any[]) {
        this.toggleBlockUI(true);
        this.postCategoryService.deletePostCategory(ids).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
            next: () => {
                this.alertService.showSuccess(MessageConstants.DELETED_OK_MSG);
                this.selectedItems = [];
                this.loadData();
                this.toggleBlockUI(false);
            },
            error: () => {
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
        }
        else {
            setTimeout(() => {
                this.blockedPanel = false;
            }, 1000);
        }
    }
}