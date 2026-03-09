import { useState, useEffect, useCallback, useMemo, useRef } from 'react';
import {
  TableView,
  TableHeader,
  Column,
  TableBody,
  Row,
  Cell,
  Badge,
  TagGroup,
  Tag,
  ActionButton,
  Button,
  SearchField,
  ComboBox,
  Picker,
  PickerItem,
  Text,
  IllustratedMessage,
  Heading,
  Content,
  ProgressCircle,
  InlineAlert,
  Dialog,
  DialogTrigger,
  useDialogContainer,
  AlertDialog,
  Divider,
} from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import { useTranslation } from '../../../i18n/useTranslation';
import { createQuestionService } from '../../../services/questionService';
import { useAuth } from '../../../contexts/AuthContext';
import Edit from '@react-spectrum/s2/icons/Edit';
import Delete from '@react-spectrum/s2/icons/Delete';
import Add from '@react-spectrum/s2/icons/Add';
import Search from '@react-spectrum/s2/icons/Search';
import CheckmarkCircle from '@react-spectrum/s2/icons/CheckmarkCircle';
import FolderOpen from '@react-spectrum/s2/illustrations/linear/FolderOpen';

// Styles
const containerStyle = style({
  display: 'flex',
  flexDirection: 'column',
  flexGrow: 1,
  gap: 16,
});

const headerRowStyle = style({
  display: 'flex',
  flexDirection: 'row',
  justifyContent: 'space-between',
  alignItems: 'center',
  flexWrap: 'wrap',
  gap: 12,
});

const filtersRowStyle = style({
  display: 'flex',
  flexDirection: 'row',
  alignItems: 'center',
  flexWrap: 'wrap',
  gap: 12,
});

const paginationRowStyle = style({
  display: 'flex',
  flexDirection: 'row',
  justifyContent: 'space-between',
  alignItems: 'center',
  paddingTop: 16,
  flexWrap: 'wrap',
  gap: 12,
});

const paginationLinksStyle = style({
  display: 'flex',
  flexDirection: 'row',
  alignItems: 'center',
  gap: 4,
});

const tagGroupStyle = style({
  display: 'flex',
  flexWrap: 'wrap',
  gap: 4,
});

const actionButtonsStyle = style({
  display: 'flex',
  flexDirection: 'row',
  gap: 4,
});

const pageInfoStyle = style({
  font: 'body-sm',
  color: 'neutral-subdued',
});

const pageSizeLabelStyle = style({
  font: 'body-sm',
  color: 'neutral-subdued',
});

/**
 * Delete confirmation dialog
 */
function DeleteDialog({ questionId, questionNumber, onDelete, onClose }) {
  const { t } = useTranslation();
  const [isDeleting, setIsDeleting] = useState(false);
  const dialog = useDialogContainer();

  const handleDelete = async () => {
    setIsDeleting(true);
    try {
      await onDelete(questionId);
      dialog.dismiss(true);
    } catch (error) {
      // Error is handled by parent
    } finally {
      setIsDeleting(false);
    }
  };

  return (
    <AlertDialog
      title={t('admin.questions.table.confirmDeleteTitle')}
      variant="destructive"
      primaryActionLabel={isDeleting ? t('common.loading') : t('admin.questions.table.delete')}
      cancelLabel={t('common.cancel')}
      onPrimaryAction={handleDelete}
      isPrimaryActionDisabled={isDeleting}
      onCancel={dialog.dismiss}
    >
      <Text>{t('admin.questions.table.confirmDelete')}</Text>
      <Text weight="bold">{questionNumber}</Text>
    </AlertDialog>
  );
}

/**
 * QuestionsTable - A table view component for managing questions
 */
export default function QuestionsTable() {
  const { t } = useTranslation();
  const { getToken, onUnauthorized } = useAuth();

  // Create service ref to avoid recreation
  const questionServiceRef = useRef(null);
  if (!questionServiceRef.current) {
    questionServiceRef.current = createQuestionService({ getToken, onUnauthorized });
  }
  const questionService = questionServiceRef.current;

  // State
  const [questions, setQuestions] = useState([]);
  const [totalCount, setTotalCount] = useState(0);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [searchText, setSearchText] = useState('');
  const [searchDebounced, setSearchDebounced] = useState('');
  const [selectedTags, setSelectedTags] = useState([]);
  const [tagInput, setTagInput] = useState('');
  const [selectedRows, setSelectedRows] = useState(new Set());
  const [loadingState, setLoadingState] = useState('idle');
  const [error, setError] = useState(null);
  const [availableTags, setAvailableTags] = useState([]);
  const [tagsLoading, setTagsLoading] = useState(false);

  // Calculate total pages
  const totalPages = Math.ceil(totalCount / pageSize);

  // Fetch questions
  const fetchQuestions = useCallback(async (resetPage = false) => {
    setLoadingState('loading');
    setError(null);

    const page = resetPage ? 1 : pageNumber;
    if (resetPage) {
      setPageNumber(1);
    }

    try {
      const tagsParam = selectedTags.length > 0
        ? selectedTags.map(t => t.name).join(',')
        : undefined;

      const response = await questionService.listQuestions({
        pageNumber: page,
        pageSize,
        searchText: searchDebounced || undefined,
        tags: tagsParam,
      });

      setQuestions(response.items || []);
      setTotalCount(response.totalCount || 0);
      setLoadingState('idle');
    } catch (err) {
      console.error('Failed to fetch questions:', err);
      setError(t('admin.questions.table.loadingError'));
      setLoadingState('idle');
    }
  }, [pageNumber, pageSize, searchDebounced, selectedTags, questionService, t]);

  // Search tags for autocomplete
  const searchTags = useCallback(async (search) => {
    setTagsLoading(true);
    try {
      const tags = await questionService.searchTags(search);
      setAvailableTags(tags.slice(0, 5));
    } catch (err) {
      console.error('Failed to search tags:', err);
    } finally {
      setTagsLoading(false);
    }
  }, [questionService]);

  // Debounced search
  useEffect(() => {
    const timer = setTimeout(() => {
      setSearchDebounced(searchText);
    }, 300);
    return () => clearTimeout(timer);
  }, [searchText]);

  // Fetch on filter/page change
  useEffect(() => {
    fetchQuestions();
  }, [searchDebounced, selectedTags, pageSize, pageNumber]); // eslint-disable-line react-hooks/exhaustive-deps

  // Initial tag load
  useEffect(() => {
    searchTags('');
  }, [searchTags]);

  // Handle page change
  const handlePageChange = (newPage) => {
    if (newPage >= 1 && newPage <= totalPages) {
      setPageNumber(newPage);
    }
  };

  // Handle delete
  const handleDelete = async (questionId) => {
    await questionService.deleteQuestion(questionId);
    fetchQuestions();
  };

  // Calculate display range
  const startItem = (pageNumber - 1) * pageSize + 1;
  const endItem = Math.min(pageNumber * pageSize, totalCount);

  // Format date
  const formatDate = (dateString) => {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleDateString();
  };

  // Page size options
  const pageSizeOptions = useMemo(() => [
    { id: 10, name: '10' },
    { id: 20, name: '20' },
    { id: 50, name: '50' },
  ], []);

  // Generate page numbers to display
  const pageNumbers = useMemo(() => {
    const pages = [];
    const maxVisible = 5;
    let start = Math.max(1, pageNumber - Math.floor(maxVisible / 2));
    let end = Math.min(totalPages, start + maxVisible - 1);

    if (end - start < maxVisible - 1) {
      start = Math.max(1, end - maxVisible + 1);
    }

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return pages;
  }, [pageNumber, totalPages]);

  // Handle search change
  const handleSearchChange = (value) => {
    setSearchText(value);
    setPageNumber(1); // Reset to first page on search
  };

  // Handle page size change
  const handlePageSizeChange = (size) => {
    setPageSize(size);
    setPageNumber(1);
  };

  // Render empty state
  const renderEmptyState = useCallback(() => (
    <IllustratedMessage>
      <FolderOpen />
      <Heading>{t('admin.questions.table.noResults')}</Heading>
      <Content>{t('admin.questions.table.noResultsDescription')}</Content>
    </IllustratedMessage>
  ), [t]);

  return (
    <div className={containerStyle}>
      {/* Header row with Add button */}
      <div className={headerRowStyle}>
        <div className={filtersRowStyle}>
          {/* Search field */}
          <SearchField
            placeholder={t('admin.questions.table.searchPlaceholder')}
            value={searchText}
            onChange={handleSearchChange}
            styles={style({ minWidth: 200, maxWidth: 300 })}
          >
            <Search />
          </SearchField>

          {/* Tags filter */}
          <ComboBox
            placeholder={t('admin.questions.table.filterByTags')}
            items={availableTags}
            selectedKey={null}
            inputValue={tagInput}
            onInputChange={(value) => {
              setTagInput(value);
              searchTags(value);
            }}
            onSelectionChange={(key) => {
              if (key) {
                const tag = availableTags.find(t => t.id === key);
                if (tag && !selectedTags.find(t => t.id === key)) {
                  setSelectedTags(prev => [...prev, tag]);
                  setTagInput('');
                  setPageNumber(1);
                }
              }
            }}
            isLoading={tagsLoading}
            allowsCustomValue={false}
            styles={style({ minWidth: 150 })}
          >
            {item => <PickerItem>{item.name}</PickerItem>}
          </ComboBox>

          {/* Selected tags display */}
          {selectedTags.length > 0 && (
            <TagGroup
              aria-label="Selected tags"
              items={selectedTags}
              onRemove={(keys) => {
                setSelectedTags(prev => prev.filter(t => !keys.has(t.id)));
                setPageNumber(1);
              }}
              styles={tagGroupStyle}
            >
              {item => <Tag key={item.id}>{item.name}</Tag>}
            </TagGroup>
          )}

        </div>

        {/* Add Question button */}
        <Button variant="accent" onPress={() => {/* TODO: Navigate to add question */ }}>
          <Add />
          <Text>{t('admin.questions.table.addQuestion')}</Text>
        </Button>
      </div>

      {/* Error alert */}
      {error && (
        <InlineAlert variant="negative">
          <Heading>{error}</Heading>
          <ActionButton onPress={() => fetchQuestions()}>
            {t('admin.questions.table.retry')}
          </ActionButton>
        </InlineAlert>
      )}

      {/* Table */}
      <TableView
        aria-label={t('admin.questions.title')}
        selectionMode="multiple"
        selectedKeys={selectedRows}
        onSelectionChange={setSelectedRows}
        loadingState={loadingState}
        density="compact"
        styles={style({ width: 'full', minHeight: 100 })}
        overflowMode="truncate"
      >
        <TableHeader>
          <Column id="number" width={120} allowsResizing isRowHeader>
            {t('admin.questions.table.questionNumber')}
          </Column>
          <Column id="text" defaultWidth="2fr" allowsResizing>
            {t('admin.questions.table.questionText')}
          </Column>
          <Column id="tags" width={250} allowsResizing>
            {t('admin.questions.table.tags')}
          </Column>
          <Column id="createdAt" width={100} allowsResizing>
            {t('admin.questions.table.createdAt')}
          </Column>
          <Column id="actions" width={80}>
            {t('admin.questions.table.actions')}
          </Column>
        </TableHeader>
        <TableBody
          items={questions}
          renderEmptyState={loadingState === 'idle' && !error ? renderEmptyState : undefined}
        >
          {(item) => (
            <Row id={item.id}>
              <Cell>
                <div className={style({ display: 'flex', flexDirection: 'row', alignItems: 'center', gap: 8 })}>
                  {item.isVerified && (
                    <CheckmarkCircle UNSAFE_style={{ color: 'var(--spectrum-positive-color)' }} />
                  )}
                  <span>{item.number}</span>
                </div>
              </Cell>
              <Cell>
                <span className={style({ display: 'block', truncate: true })}>
                  {item.text}
                </span>
              </Cell>
              <Cell>
                {item.tags && item.tags.length > 0 ? (
                  <div className={tagGroupStyle}>
                    {(() => {
                      const selectedIds = new Set(selectedTags.map(t => t.id));
                      const sortedTags = [...item.tags].sort((a, b) => {
                        const aSelected = selectedIds.has(a.id);
                        const bSelected = selectedIds.has(b.id);
                        const aMatches = searchText && a.name.toLowerCase().includes(searchText.toLowerCase());
                        const bMatches = searchText && b.name.toLowerCase().includes(searchText.toLowerCase());

                        const aPrioritized = aSelected || aMatches;
                        const bPrioritized = bSelected || bMatches;

                        if (aPrioritized && !bPrioritized) return -1;
                        if (!aPrioritized && bPrioritized) return 1;
                        return a.name.localeCompare(b.name);
                      });

                      return sortedTags.map(tag => (
                        <Badge key={tag.id} variant={selectedIds.has(tag.id) ? 'accent' : 'neutral'}>
                          {tag.name}
                        </Badge>
                      ));
                    })()}
                  </div>
                ) : (
                  <Text color="neutral-subdued">-</Text>
                )}
              </Cell>
              <Cell>{formatDate(item.createdAt)}</Cell>
              <Cell>
                <div className={actionButtonsStyle}>
                  <ActionButton
                    isQuiet
                    size="S"
                    aria-label={t('admin.questions.table.edit')}
                    onPress={() => {/* TODO: Navigate to edit */ }}
                  >
                    <Edit />
                  </ActionButton>
                  <DialogTrigger>
                    <ActionButton
                      isQuiet
                      size="S"
                      aria-label={t('admin.questions.table.delete')}
                    >
                      <Delete />
                    </ActionButton>
                    {(close) => (
                      <DeleteDialog
                        questionId={item.id}
                        questionNumber={item.number}
                        onDelete={handleDelete}
                        onClose={close}
                      />
                    )}
                  </DialogTrigger>
                </div>
              </Cell>
            </Row>
          )}
        </TableBody>
      </TableView>

      {/* Pagination */}
      <div className={paginationRowStyle}>
        {/* Page info */}
        <span className={pageInfoStyle}>
          {totalCount > 0
            ? t('admin.questions.table.showing')
              .replace('{start}', startItem)
              .replace('{end}', endItem)
              .replace('{total}', totalCount)
            : t('admin.questions.table.noResults')}
        </span>

        {/* Page size selector */}
        <div className={style({ display: 'flex', flexDirection: 'row', alignItems: 'center', gap: 8 })}>
          <span className={pageSizeLabelStyle}>{t('admin.questions.table.pageSize')}</span>
          <Picker
            items={pageSizeOptions}
            selectedKey={pageSize}
            onSelectionChange={handlePageSizeChange}
            styles={style({ minWidth: 80 })}
          >
            {item => <PickerItem>{item.name}</PickerItem>}
          </Picker>
        </div>

        {/* Page navigation */}
        {totalPages > 1 && (
          <div className={paginationLinksStyle}>
            <ActionButton
              isQuiet
              size="S"
              isDisabled={pageNumber <= 1}
              onPress={() => handlePageChange(1)}
            >
              {'<<'}
            </ActionButton>
            <ActionButton
              isQuiet
              size="S"
              isDisabled={pageNumber <= 1}
              onPress={() => handlePageChange(pageNumber - 1)}
            >
              {'<'}
            </ActionButton>

            {pageNumbers.map(page => (
              <ActionButton
                key={page}
                size="S"
                variant={page === pageNumber ? 'accent' : 'default'}
                isQuiet={page !== pageNumber}
                onPress={() => handlePageChange(page)}
              >
                {page}
              </ActionButton>
            ))}

            <ActionButton
              isQuiet
              size="S"
              isDisabled={pageNumber >= totalPages}
              onPress={() => handlePageChange(pageNumber + 1)}
            >
              {'>'}
            </ActionButton>
            <ActionButton
              isQuiet
              size="S"
              isDisabled={pageNumber >= totalPages}
              onPress={() => handlePageChange(totalPages)}
            >
              {'>>'}
            </ActionButton>
          </div>
        )}
      </div>
    </div>
  );
}