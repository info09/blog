import { PostSeriesComponent } from './post-series.component';
import { PostDetailComponent } from './post-detail.component';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiPostApiClient, AdminApiPostCategoryApiClient, AdminApiTestApiClient, PostCategoryDto, PostDto, PostInListDto, PostInListDtoPagedResult } from 'src/app/api/admin-api.service.generated';
import { MessageConstants } from 'src/app/shared/constants/message.constants';
import { AlertService } from 'src/app/shared/services/alert.service';
import { PostReturnReasonComponent } from './post-return-reason.component';
import { PostActivityLogComponent } from './post-activity-log.component';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html'
})
export class PostComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  public blockedPanel: boolean = false;

  //Paging variables
  public pageIndex: number = 1;
  public pageSize: number = 10;
  public totalCount: number;

  //Business variables
  public items: PostInListDto[];
  public selectedItems: PostInListDto[] = [];
  public keyword: string = '';

  public categoryId?: string = null;
  public postCategories: any[] = [];

  constructor(private potsCategoryService: AdminApiPostCategoryApiClient, private postService: AdminApiPostApiClient, public dialogService: DialogService, private alertService: AlertService, private confirmationService: ConfirmationService) { }
  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
  ngOnInit(): void {
    this.loadPostCategory();
    this.loadData();
  }

  loadData() {
    this.toggleBlockUI(true);
    this.postService.getPostsPaging(this.keyword, this.categoryId, this.pageIndex, this.pageSize).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
      next: (res: PostInListDtoPagedResult) => {
        this.items = res.results;
        this.totalCount = res.rowCount;
        this.toggleBlockUI(false);
      },
      error: () => {
        this.toggleBlockUI(false);
      }
    })
  }

  loadPostCategory() {
    this.potsCategoryService.getAllPostCategories().pipe(takeUntil(this.ngUnsubscribe)).subscribe({
      next: (res: PostCategoryDto[]) => {
        res.forEach(element => {
          this.postCategories.push({
            value: element.id,
            label: element.name
          })
        });
      }
    })
  }
  showAddModal() {
    const ref = this.dialogService.open(PostDetailComponent, {
      header: 'Thêm mới bài viết',
      width: '70%'
    });
    ref.onClose.subscribe((data: PostDto) => {
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
    const ref = this.dialogService.open(PostDetailComponent, {
      data: { id: id },
      header: 'Cập nhật bài viết',
      width: '70%'
    });
    ref.onClose.subscribe((data: PostDto) => {
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
    this.selectedItems.forEach(el => ids.push(el.id));
    this.confirmationService.confirm({
      message: MessageConstants.CONFIRM_DELETE_MSG,
      accept: () => this.deleteItemsConfirm(ids)
    })
  }
  deleteItemsConfirm(ids: any) {
    this.toggleBlockUI(true);
    this.postService.deletePosts(ids).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
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
  addToSeries(id: string) {
    const ref = this.dialogService.open(PostSeriesComponent, {
      data: { id: id },
      header: 'Thêm vào loạt bài',
      width: '70%'
    });
    ref.onClose.subscribe((data: PostDto) => {
      if (data) {
        this.alertService.showSuccess(MessageConstants.CREATED_OK_MSG);
        this.selectedItems = [];
        this.loadData();
      }
    })
  }
  approve(id: string) {
    this.confirmationService.confirm({
      message: 'Bạn có xác nhận publish bài viết?',
      accept: () => this.approveConfirm(id)
    })
  }

  approveConfirm(id: string) {
    this.toggleBlockUI(true);
    this.postService.approvePost(id).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
      next: () => {
        this.alertService.showSuccess(MessageConstants.UPDATED_OK_MSG);
        this.selectedItems = [];
        this.loadData();
        this.toggleBlockUI(false);
      },
      error: () => {
        this.toggleBlockUI(false);
      }
    })
  }
  sendToApprove(id: string) {
    this.confirmationService.confirm({
      message: 'Bạn có muốn gửi duyệt bài viết?',
      accept: () => this.sendToApproveConfirm(id)
    })
  }
  sendToApproveConfirm(id: string) {
    this.toggleBlockUI(true);
    this.postService.sendToApprove(id).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
      next: () => {
        this.alertService.showSuccess(MessageConstants.UPDATED_OK_MSG);
        this.selectedItems = [];
        this.loadData();
        this.toggleBlockUI(false);
      },
      error: () => {
        this.toggleBlockUI(false);
      }
    })
  }
  reject(id: string) {
    const ref = this.dialogService.open(PostReturnReasonComponent, {
      data: { id: id },
      header: 'Trả lại bài viết',
      width: '70%'
    });
    ref.onClose.subscribe((data: PostDto) => {
      if (data) {
        this.alertService.showSuccess(MessageConstants.CREATED_OK_MSG);
        this.selectedItems = [];
        this.loadData();
      }
    })
  }
  showLogs(id: string) {
    const ref = this.dialogService.open(PostActivityLogComponent, {
      data: { id: id },
      header: 'Xem lịch sử',
      width: '70%'
    });
    ref.onClose.subscribe((data: PostDto) => {
      if (data) {
        this.alertService.showSuccess(MessageConstants.CREATED_OK_MSG);
        this.selectedItems = [];
        this.loadData();
      }
    })
  }

  pageChanged(event: any): void {
    this.pageIndex = event.page + 1;
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