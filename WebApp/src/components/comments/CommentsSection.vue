<template>
  <section class="rounded-lg border border-slate-200 bg-white shadow-sm">
    <div class="flex flex-col gap-2 border-b border-slate-200 px-5 py-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <h2 class="text-lg font-semibold text-slate-900">Comments</h2>
        <p class="text-sm text-slate-500">{{ comments?.length || 0 }} total on this item</p>
      </div>
      <button
        v-if="hiddenCommentCount > 0"
        type="button"
        class="rounded-md border border-slate-300 px-3 py-2 text-sm font-semibold text-slate-700 transition hover:bg-slate-50"
        @click="showAll = !showAll"
      >
        {{ showAll ? 'Show less' : `See ${hiddenCommentCount} more` }}
      </button>
    </div>

    <div class="space-y-4 p-5">
      <form class="rounded-lg border border-slate-200 bg-slate-50 p-4" @submit.prevent="submitComment">
        <label class="block text-sm font-semibold text-slate-700" :for="commentInputId">Add comment</label>
        <div class="relative mt-2">
          <textarea
            :id="commentInputId"
            ref="newTextarea"
            v-model="newCommentBody"
            class="min-h-24 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 outline-none transition placeholder:text-slate-400 focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
            placeholder="Write an update, decision, or blocker..."
            @click="refreshMentionState('new')"
            @input="refreshMentionState('new')"
            @keydown.esc="closeMentionPicker"
          ></textarea>

          <div
            v-if="activeMentionMode === 'new' && filteredMentionUsers.length"
            class="absolute left-0 top-full z-20 mt-1 max-h-56 w-full max-w-md overflow-auto rounded-md border border-slate-200 bg-white py-1 shadow-lg"
          >
            <button
              v-for="user in filteredMentionUsers"
              :key="user.userId"
              type="button"
              class="flex w-full items-center gap-3 px-3 py-2 text-left text-sm hover:bg-blue-50"
              @mousedown.prevent="selectMention(user, 'new')"
            >
              <span class="grid size-8 place-items-center rounded-full bg-slate-900 text-xs font-bold text-white">
                {{ initials(user.username) }}
              </span>
              <span class="min-w-0">
                <span class="block truncate font-semibold text-slate-900">{{ user.username }}</span>
                <span class="block truncate text-xs text-slate-500">{{ user.email }}</span>
              </span>
            </button>
          </div>
        </div>

        <div class="mt-3 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div class="flex min-h-9 flex-wrap gap-2">
            <span
              v-for="userId in newMentionUserIds"
              :key="userId"
              class="inline-flex items-center gap-1 rounded-full bg-blue-50 px-2.5 py-1 text-xs font-semibold text-blue-700"
            >
              @{{ userName(userId) }}
              <button type="button" class="text-blue-500 hover:text-blue-800" @click="removeMention(userId, 'new')">
                x
              </button>
            </span>
          </div>
          <button
            type="submit"
            class="rounded-md bg-blue-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:bg-slate-300"
            :disabled="!newCommentBody.trim() || createCommentMutation.isPending.value"
          >
            Post comment
          </button>
        </div>
      </form>

      <div v-if="isLoading" class="rounded-lg border border-dashed border-slate-300 p-5 text-sm text-slate-500">
        Loading comments...
      </div>

      <div v-else-if="!comments?.length" class="rounded-lg border border-dashed border-slate-300 p-5 text-sm text-slate-500">
        No comments yet.
      </div>

      <div v-else class="space-y-3">
        <article
          v-for="comment in visibleComments"
          :key="comment.id"
          class="rounded-lg border border-slate-200 bg-white p-4"
        >
          <div class="flex flex-col gap-2 sm:flex-row sm:items-start sm:justify-between">
            <div class="min-w-0">
              <div class="flex flex-wrap items-center gap-2">
                <span class="grid size-8 place-items-center rounded-full bg-slate-900 text-xs font-bold text-white">
                  {{ initialsForUser(comment.createdByUserId) }}
                </span>
                <div>
                  <p class="text-sm font-semibold text-slate-900">{{ userName(comment.createdByUserId) }}</p>
                  <p class="text-xs text-slate-500">
                    {{ formatDate(comment.createdAt) }}
                    <span v-if="comment.updatedAt"> · edited {{ formatDate(comment.updatedAt) }}</span>
                  </p>
                </div>
              </div>
            </div>
            <div class="flex gap-2">
              <button
                type="button"
                class="rounded-md border border-slate-300 px-3 py-1.5 text-xs font-semibold text-slate-700 hover:bg-slate-50"
                @click="startEdit(comment)"
              >
                Edit
              </button>
              <button
                type="button"
                class="rounded-md border border-red-200 px-3 py-1.5 text-xs font-semibold text-red-700 hover:bg-red-50"
                @click="removeComment(comment.id)"
              >
                Delete
              </button>
            </div>
          </div>

          <div v-if="editingCommentId === comment.id" class="mt-4 space-y-3">
            <div class="relative">
              <textarea
                ref="editTextarea"
                v-model="editBody"
                class="min-h-20 w-full rounded-md border border-slate-300 px-3 py-2 text-sm text-slate-900 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
                @click="refreshMentionState('edit')"
                @input="refreshMentionState('edit')"
                @keydown.esc="closeMentionPicker"
              ></textarea>

              <div
                v-if="activeMentionMode === 'edit' && filteredMentionUsers.length"
                class="absolute left-0 top-full z-20 mt-1 max-h-56 w-full max-w-md overflow-auto rounded-md border border-slate-200 bg-white py-1 shadow-lg"
              >
                <button
                  v-for="user in filteredMentionUsers"
                  :key="user.userId"
                  type="button"
                  class="flex w-full items-center gap-3 px-3 py-2 text-left text-sm hover:bg-blue-50"
                  @mousedown.prevent="selectMention(user, 'edit')"
                >
                  <span class="grid size-8 place-items-center rounded-full bg-slate-900 text-xs font-bold text-white">
                    {{ initials(user.username) }}
                  </span>
                  <span class="min-w-0">
                    <span class="block truncate font-semibold text-slate-900">{{ user.username }}</span>
                    <span class="block truncate text-xs text-slate-500">{{ user.email }}</span>
                  </span>
                </button>
              </div>
            </div>
            <div class="flex min-h-8 flex-wrap gap-2">
              <span
                v-for="userId in editMentionUserIds"
                :key="userId"
                class="inline-flex items-center gap-1 rounded-full bg-blue-50 px-2.5 py-1 text-xs font-semibold text-blue-700"
              >
                @{{ userName(userId) }}
                <button type="button" class="text-blue-500 hover:text-blue-800" @click="removeMention(userId, 'edit')">
                  x
                </button>
              </span>
            </div>
            <div class="flex gap-2">
              <button
                type="button"
                class="rounded-md bg-blue-600 px-3 py-2 text-sm font-semibold text-white hover:bg-blue-700 disabled:cursor-not-allowed disabled:bg-slate-300"
                :disabled="!editBody.trim() || updateCommentMutation.isPending.value"
                @click="saveEdit(comment.id)"
              >
                Save
              </button>
              <button
                type="button"
                class="rounded-md border border-slate-300 px-3 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50"
                @click="cancelEdit"
              >
                Cancel
              </button>
            </div>
          </div>

          <div v-else class="mt-4">
            <p class="whitespace-pre-wrap text-sm leading-6 text-slate-700">{{ comment.body }}</p>
            <div v-if="comment.mentionedUserIds?.length" class="mt-3 flex flex-wrap gap-2">
              <span
                v-for="userId in comment.mentionedUserIds"
                :key="userId"
                class="rounded-full bg-blue-50 px-2.5 py-1 text-xs font-semibold text-blue-700"
              >
                @{{ userName(userId) }}
              </span>
            </div>
          </div>
        </article>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed, nextTick, ref } from "vue";
import { useUsers } from "../../composables/useUsers";
import {
  useComments,
  useCreateComment,
  useDeleteComment,
  useUpdateComment,
} from "../../composables/useComments";
import type { CommentResponse, CommentEntityType } from "../../services/commentApi";

type MentionMode = "new" | "edit";
type UserOption = {
  userId: string;
  username: string;
  email: string;
};

const props = defineProps<{
  entityId: string;
  entityType: CommentEntityType;
}>();

const { data: users } = useUsers();
const { data: comments, isLoading } = useComments(props.entityId, props.entityType);
const createCommentMutation = useCreateComment();
const updateCommentMutation = useUpdateComment();
const deleteCommentMutation = useDeleteComment();

const showAll = ref(false);
const newCommentBody = ref("");
const newMentionUserIds = ref<string[]>([]);
const editingCommentId = ref("");
const editBody = ref("");
const editMentionUserIds = ref<string[]>([]);
const commentInputId = `comment-input-${props.entityType}-${props.entityId}`;
const newTextarea = ref<HTMLTextAreaElement | null>(null);
const editTextarea = ref<HTMLTextAreaElement | HTMLTextAreaElement[] | null>(null);
const activeMentionMode = ref<MentionMode | "">("");
const mentionQuery = ref("");
const mentionStart = ref(0);
const mentionEnd = ref(0);

const visibleComments = computed(() => {
  const items = comments.value || [];
  return showAll.value ? items : items.slice(0, 3);
});

const hiddenCommentCount = computed(() => Math.max((comments.value?.length || 0) - 3, 0));

const selectedMentionUserIds = computed(() => {
  return activeMentionMode.value === "edit" ? editMentionUserIds.value : newMentionUserIds.value;
});

const filteredMentionUsers = computed<UserOption[]>(() => {
  const query = mentionQuery.value.toLowerCase();
  const selected = new Set(selectedMentionUserIds.value);

  return ((users.value || []) as UserOption[])
    .filter((user) => !selected.has(user.userId))
    .filter((user) => {
      if (!query) return true;

      return (
        user.username.toLowerCase().includes(query) ||
        user.email.toLowerCase().includes(query)
      );
    })
    .slice(0, 8);
});

const submitComment = async () => {
  const body = newCommentBody.value.trim();
  if (!body) return;

  await createCommentMutation.mutateAsync({
    entityId: props.entityId,
    entityType: props.entityType,
    body,
    mentionUserIds: newMentionUserIds.value,
  });

  newCommentBody.value = "";
  newMentionUserIds.value = [];
  closeMentionPicker();
};

const startEdit = (comment: CommentResponse) => {
  editingCommentId.value = comment.id;
  editBody.value = comment.body;
  editMentionUserIds.value = [...(comment.mentionedUserIds || [])];
};

const cancelEdit = () => {
  editingCommentId.value = "";
  editBody.value = "";
  editMentionUserIds.value = [];
  closeMentionPicker();
};

const saveEdit = async (commentId: string) => {
  const body = editBody.value.trim();
  if (!body) return;

  await updateCommentMutation.mutateAsync({
    commentId,
    data: {
      entityId: props.entityId,
      entityType: props.entityType,
      body,
      mentionUserIds: editMentionUserIds.value,
    },
  });

  cancelEdit();
};

const refreshMentionState = (mode: MentionMode) => {
  const textarea = getTextarea(mode);
  if (!textarea) return;

  const cursor = textarea.selectionStart ?? textarea.value.length;
  const textBeforeCursor = textarea.value.slice(0, cursor);
  const mentionMatch = textBeforeCursor.match(/(?:^|\s)@([^\s@]*)$/);

  if (!mentionMatch) {
    if (activeMentionMode.value === mode) closeMentionPicker();
    return;
  }

  activeMentionMode.value = mode;
  mentionQuery.value = mentionMatch[1] ?? "";
  mentionStart.value = textBeforeCursor.lastIndexOf("@");
  mentionEnd.value = cursor;
};

const selectMention = async (user: UserOption, mode: MentionMode) => {
  const body = mode === "new" ? newCommentBody : editBody;
  const mentionIds = mode === "new" ? newMentionUserIds : editMentionUserIds;
  const insertText = `@${user.username} `;
  const nextBody =
    body.value.slice(0, mentionStart.value) +
    insertText +
    body.value.slice(mentionEnd.value);

  body.value = nextBody;

  if (!mentionIds.value.includes(user.userId)) {
    mentionIds.value = [...mentionIds.value, user.userId];
  }

  closeMentionPicker();

  await nextTick();
  const textarea = getTextarea(mode);
  const nextCursor = mentionStart.value + insertText.length;
  textarea?.setSelectionRange(nextCursor, nextCursor);
  textarea?.focus();
};

const removeMention = (userId: string, mode: MentionMode) => {
  const mentionIds = mode === "new" ? newMentionUserIds : editMentionUserIds;
  mentionIds.value = mentionIds.value.filter((id) => id !== userId);
};

const closeMentionPicker = () => {
  activeMentionMode.value = "";
  mentionQuery.value = "";
};

const getTextarea = (mode: MentionMode) => {
  if (mode === "new") return newTextarea.value;

  return Array.isArray(editTextarea.value) ? editTextarea.value[0] : editTextarea.value;
};

const removeComment = async (commentId: string) => {
  await deleteCommentMutation.mutateAsync({
    commentId,
    entityId: props.entityId,
    entityType: props.entityType,
  });
};

const userName = (userId: string) => {
  const user = users.value?.find((item: any) => item.userId === userId);
  return user?.username || "User";
};

const initialsForUser = (userId: string) => {
  return initials(userName(userId));
};

const initials = (name: string) => {
  return name
    .split(" ")
    .map((part: string) => part[0])
    .join("")
    .slice(0, 2)
    .toUpperCase();
};

const formatDate = (value: string) =>
  new Date(value).toLocaleString(undefined, {
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
</script>
