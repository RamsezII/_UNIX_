using _UNIX_;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _UNIX_
{
    public partial class TERMINAL
    {
        Canvas canvas;
        RectTransform prompt_rT, viewport_rT, text_zone_rT, tasks1_rT, tasks2_rT;
        Graphic background_img;
        TEXTFIELD output_tf, label_tf, input_tf, nano_tf;
        TextMeshProUGUI title, tasks1_text, tasks2_text, completion_text;
        Scrollbar scrollbar;
        DRAGGABLE drag_top, drag_right, draw_bottom, draw_left, left_corner, right_corner;

        [SerializeField, Range(0, 1)] float ui_hue;
        [SerializeField] float ui_alpha;
        [SerializeField] float ui_hue_speed = .03f;

        //----------------------------------------------------------------------------------------------------------

        void AwakeUI()
        {
            canvas = GetComponentInChildren<Canvas>();

            foreach (var rt in transform.GetComponentsInChildren<RectTransform>(true))
                if (rt.gameObject.CompareTag("EditorOnly"))
                    rt.gameObject.SetActive(false);

            prompt_rT = (RectTransform)transform;
            viewport_rT = (RectTransform)transform.Find("body/Scroll View/Viewport");
            text_zone_rT = (RectTransform)transform.Find("body/Scroll View/Viewport/TEXT_ZONE");

            output_tf = transform.Find("body/Scroll View/Viewport/TEXT_ZONE/OUTPUT").GetComponent<TEXTFIELD>();
            label_tf = transform.Find("body/Scroll View/Viewport/TEXT_ZONE/LABEL").GetComponent<TEXTFIELD>();
            input_tf = transform.Find("body/Scroll View/Viewport/TEXT_ZONE/INPUT").GetComponent<TEXTFIELD>();

            output_tf.OnAwake();
            label_tf.OnAwake();
            input_tf.OnAwake();

            tasks1_rT = (RectTransform)transform.Find("body/Scroll View/Viewport/TEXT_ZONE/TASKS1");
            tasks1_text = tasks1_rT.GetComponent<TextMeshProUGUI>();
            tasks1_text.text = string.Empty;

            tasks2_rT = (RectTransform)transform.Find("body/Scroll View/Viewport/TEXT_ZONE/TASKS2");
            tasks2_text = tasks2_rT.GetComponent<TextMeshProUGUI>();
            tasks2_text.text = string.Empty;

            completion_text = transform.Find("body/Scroll View/Viewport/TEXT_ZONE/INPUT/Text Area/COMPLETION").GetComponent<TextMeshProUGUI>();
            completion_text.text = string.Empty;

            scrollbar = transform.Find("body/Scroll View/Scrollbar Vertical").GetComponent<Scrollbar>();

            background_img = transform.Find("background").GetComponent<Image>();
            ui_alpha = background_img.color.a;

            drag_top = transform.Find("SELECT_TOP").GetComponent<DRAGGABLE>();
            drag_right = transform.Find("SELECT_RIGHT").GetComponent<DRAGGABLE>();
            draw_bottom = transform.Find("SELECT_BOTTOM").GetComponent<DRAGGABLE>();
            draw_left = transform.Find("SELECT_LEFT").GetComponent<DRAGGABLE>();
            left_corner = transform.Find("CORNER_LEFT").GetComponent<DRAGGABLE>();
            right_corner = transform.Find("CORNER_RIGHT").GetComponent<DRAGGABLE>();
            drag_top.onDrag = drag_right.onDrag = draw_bottom.onDrag = draw_left.onDrag = left_corner.onDrag = right_corner.onDrag = OnDraggable;
        }

        //----------------------------------------------------------------------------------------------------------

        void UpdateColor()
        {
            ui_hue = (ui_hue + Time.unscaledDeltaTime * ui_hue_speed) % 1;
            background_img.color = background_img.color.ModifyHsv(ui_hue, ui_alpha);
        }

        //----------------------------------------------------------------------------------------------------------

        void RebuildOutput()
        {
            StringBuilder sb = new();

            lock (allLogs)
                foreach (var item in allLogs._value)
                    sb.AppendLine(item?.ToString() ?? "_NULL_".SetColor(Colors.orange));

            if (sb.Length > 0)
                output_tf.field.text = sb.ToString()[..^1];
            else
                output_tf.field.text = string.Empty;

            refreshUI.Value = true;
        }

        void OnDraggable(DRAGGABLE draggable, PointerEventData eventData)
        {
            TakeFrontWindowsFocus();

            Vector2 delta = eventData.delta / (Screen.height / 600f);

            if (draggable == drag_top)
                prompt_rT.anchoredPosition += delta;
            else if (draggable == drag_right)
            {
                prompt_rT.sizeDelta += new Vector2(delta.x, 0);
                prompt_rT.anchoredPosition += new Vector2(.5f * delta.x, 0);
            }
            else if (draggable == draw_left)
            {
                prompt_rT.sizeDelta += new Vector2(-delta.x, 0);
                prompt_rT.anchoredPosition += new Vector2(.5f * delta.x, 0);
            }
            else if (draggable == draw_bottom)
            {
                prompt_rT.sizeDelta += new Vector2(0, -delta.y);
                prompt_rT.anchoredPosition += new Vector2(0, .5f * delta.y);
            }
            else if (draggable == left_corner)
            {
                prompt_rT.sizeDelta += -delta;
                prompt_rT.anchoredPosition += .5f * new Vector2(delta.x, delta.y);
            }
            else if (draggable == right_corner)
            {
                prompt_rT.sizeDelta += new Vector2(delta.x, -delta.y);
                prompt_rT.anchoredPosition += .5f * new Vector2(delta.x, delta.y);
            }

            refreshUI.Value = true;
        }

        protected override void OnRefreshUI()
        {
            base.OnRefreshUI();

            const float MARGIN_TOP = 3, MARGIN_BOTTOM = 5;

            float viewport_h = viewport_rT.rect.size.y;

            float text_w = text_zone_rT.rect.size.x;
            float text_y = -MARGIN_TOP;

            if (!string.IsNullOrWhiteSpace(output_tf.field.text))
            {
                output_tf.rT.anchoredPosition = new(0, text_y);
                float output_h = output_tf.field.preferredHeight;
                output_tf.rT.sizeDelta = new(0, output_h);
                text_y -= output_h;
            }

            DrawTasks(tasks1_rT, tasks1_text);

            Vector2 label_dims = default;
            if (drawInput)
            {
                label_tf.rT.anchoredPosition = new(0, text_y);
                label_dims = label_tf.text.GetPreferredValues(label_tf.field.text + "█", text_w, 0);
                label_dims = label_tf.rT.sizeDelta = string.IsNullOrWhiteSpace(label_tf.field.text) ? new(0, label_dims.y) : label_dims;

                input_tf.rT.anchoredPosition = new(0, text_y);
                if (string.IsNullOrWhiteSpace(input_tf.field.text))
                {
                    input_tf.rT.sizeDelta = new(-label_dims.x, viewport_h);
                    text_y -= label_dims.y;
                }
                else
                {
                    float input_h = input_tf.text.GetPreferredValues(input_tf.field.text, text_w - label_dims.x, 0).y;
                    input_tf.rT.sizeDelta = new(-label_dims.x, input_h + viewport_h);
                    text_y -= input_h;
                }
            }

            DrawTasks(tasks2_rT, tasks2_text);

            float text_zone = -text_y + viewport_h - label_dims.y - MARGIN_TOP;
            text_zone_rT.sizeDelta = new(-15, text_zone);

            float scroll_lerp = (text_zone + text_y - MARGIN_TOP) / (text_zone - viewport_h);
            if (scroll_lerp < scrollbar.value)
                scrollbar.value = scroll_lerp;

            void DrawTasks(in RectTransform tmp_rT, in TextMeshProUGUI tmp)
            {
                if (!string.IsNullOrWhiteSpace(tmp.text))
                {
                    tmp_rT.anchoredPosition = new(0, text_y);
                    text_y -= (tmp_rT.sizeDelta = new(0, tmp.preferredHeight)).y;
                }
            }
        }
    }
}