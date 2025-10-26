using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabsBuilderApi.backend
{
    using System;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Serialization;

    // Token: 0x020003EB RID: 1003
    public class AnotherScroller : PassiveUiElement
    {
        // Token: 0x17000378 RID: 888
        // (get) Token: 0x060018AF RID: 6319 RVA: 0x0003061D File Offset: 0x0002E81D
        public override bool HandleUp
        {
            get
            {
                return true;
            }
        }

        // Token: 0x17000379 RID: 889
        // (get) Token: 0x060018B0 RID: 6320 RVA: 0x0003061D File Offset: 0x0002E81D
        public override bool HandleDown
        {
            get
            {
                return true;
            }
        }

        // Token: 0x1700037A RID: 890
        // (get) Token: 0x060018B1 RID: 6321 RVA: 0x0003061D File Offset: 0x0002E81D
        public override bool HandleDrag
        {
            get
            {
                return true;
            }
        }

        // Token: 0x1700037B RID: 891
        // (get) Token: 0x060018B2 RID: 6322 RVA: 0x00064787 File Offset: 0x00062987
        public override bool HandleOverOut
        {
            get
            {
                return this.MouseMustBeOverToScroll;
            }
        }

        // Token: 0x1700037C RID: 892
        // (get) Token: 0x060018B3 RID: 6323 RVA: 0x0006478F File Offset: 0x0006298F
        public bool AtTop
        {
            get
            {
                return this.Inner.localPosition.y <= this.ContentYBounds.min + 0.25f;
            }
        }

        // Token: 0x1700037D RID: 893
        // (get) Token: 0x060018B4 RID: 6324 RVA: 0x000647B7 File Offset: 0x000629B7
        public bool AtBottom
        {
            get
            {
                return this.Inner.localPosition.y >= this.ContentYBounds.max - 0.25f;
            }
        }

        // Token: 0x1700037E RID: 894
        // (get) Token: 0x060018B5 RID: 6325 RVA: 0x000647DF File Offset: 0x000629DF
        public bool AtLeft
        {
            get
            {
                return this.Inner.localPosition.x <= this.ContentXBounds.min + 0.25f;
            }
        }

        // Token: 0x1700037F RID: 895
        // (get) Token: 0x060018B6 RID: 6326 RVA: 0x00064807 File Offset: 0x00062A07
        public bool AtRight
        {
            get
            {
                return this.Inner.localPosition.x >= this.ContentXBounds.max - 0.25f;
            }
        }

        // Token: 0x17000380 RID: 896
        // (get) Token: 0x060018B7 RID: 6327 RVA: 0x0006482F File Offset: 0x00062A2F
        public Collider2D Hitbox
        {
            get
            {
                return this.Colliders[0];
            }
        }

        // Token: 0x060018B8 RID: 6328 RVA: 0x00064839 File Offset: 0x00062A39
        public void SetBounds(FloatRange yBounds, FloatRange xBounds)
        {
            if (yBounds != null)
            {
                this.ContentYBounds = yBounds;
            }
            if (xBounds != null)
            {
                this.ContentXBounds = xBounds;
            }
            this.UpdateScrollBars();
        }

        // Token: 0x060018B9 RID: 6329 RVA: 0x00064855 File Offset: 0x00062A55
        public void CalculateAndSetYBounds(float amount, float numPerRow, float numRowsVisible, float spacing)
        {
            this.ContentYBounds.max = Mathf.Max(0f, (float)Mathf.CeilToInt((amount - numPerRow * numRowsVisible) / numPerRow) * spacing);
            this.UpdateScrollBars();
        }

        // Token: 0x060018BA RID: 6330 RVA: 0x00064882 File Offset: 0x00062A82
        public void SetBoundsMax(float yMax, float xMax)
        {
            this.ContentYBounds.max = yMax;
            this.ContentXBounds.max = xMax;
            this.UpdateScrollBars();
        }

        // Token: 0x060018BB RID: 6331 RVA: 0x000648A2 File Offset: 0x00062AA2
        public void SetYBoundsMax(float yMax)
        {
            this.ContentYBounds.max = yMax;
            this.UpdateScrollBars();
        }

        // Token: 0x060018BC RID: 6332 RVA: 0x000648B6 File Offset: 0x00062AB6
        public void SetBoundsMin(float yMin, float xMin)
        {
            this.ContentYBounds.min = yMin;
            this.ContentXBounds.min = xMin;
            this.UpdateScrollBars();
        }

        // Token: 0x060018BD RID: 6333 RVA: 0x000648D6 File Offset: 0x00062AD6
        public void SetYBoundsMin(float yMin)
        {
            this.ContentYBounds.min = yMin;
            this.UpdateScrollBars();
        }

        // Token: 0x060018BE RID: 6334 RVA: 0x000648EA File Offset: 0x00062AEA
        public FloatRange GetYBounds()
        {
            return this.ContentYBounds;
        }

        // Token: 0x060018BF RID: 6335 RVA: 0x000648F2 File Offset: 0x00062AF2
        public FloatRange GetXBounds()
        {
            return this.ContentXBounds;
        }

        // Token: 0x060018C0 RID: 6336 RVA: 0x000648FC File Offset: 0x00062AFC
        public override void Update()
        {
            if (!this.active && this.velocity.sqrMagnitude > 0.01f)
            {
                this.velocity = Vector2.ClampMagnitude(this.velocity, this.velocity.magnitude - 10f * Time.deltaTime);
                this.ScrollRelative(this.velocity * Time.deltaTime);
                return;
            }
            if (!this.Inner)
            {
                return;
            }
            if (this.MouseMustBeOverToScroll && this.ClickMask)
            {
                Vector2 position = DestroyableSingleton<PassiveButtonManager>.Instance.controller.GetTouch(0).Position;
                this.mouseOver = this.ClickMask.OverlapPoint(position);
            }
            if (!this.MouseMustBeOverToScroll || this.mouseOver)
            {
                Vector2 vector = Input.mouseScrollDelta * this.ScrollWheelSpeed;
                if (vector.y != 0f)
                {
                    vector.y = -vector.y;
                    this.ScrollRelative(vector);
                }
                if (vector.x != 0f)
                {
                    vector.x = -vector.x;
                    this.ScrollRelative(vector);
                }
            }
        }

        // Token: 0x060018C1 RID: 6337 RVA: 0x00064A18 File Offset: 0x00062C18
        public void ScrollDown()
        {
            Collider2D collider2D = this.Colliders.First<Collider2D>();
            float num = collider2D.bounds.max.y - collider2D.bounds.min.y;
            this.ScrollRelative(new Vector2(0f, num * 0.75f));
        }

        // Token: 0x060018C2 RID: 6338 RVA: 0x00064A70 File Offset: 0x00062C70
        public void ScrollUp()
        {
            Collider2D collider2D = this.Colliders.First<Collider2D>();
            float num = collider2D.bounds.max.y - collider2D.bounds.min.y;
            this.ScrollRelative(new Vector2(0f, num * -0.75f));
        }

        // Token: 0x060018C3 RID: 6339 RVA: 0x00064AC8 File Offset: 0x00062CC8
        public float GetScrollPercY()
        {
            if (this.ContentYBounds.Width < 0.0001f)
            {
                return 1f;
            }
            Vector3 localPosition = this.Inner.transform.localPosition;
            return this.ContentYBounds.ReverseLerp(localPosition.y);
        }

        // Token: 0x060018C4 RID: 6340 RVA: 0x00064B10 File Offset: 0x00062D10
        public float GetScrollPercX()
        {
            if (this.ContentXBounds.Width < 0.0001f)
            {
                return 1f;
            }
            Vector3 localPosition = this.Inner.transform.localPosition;
            return this.ContentXBounds.ReverseLerp(localPosition.x);
        }

        // Token: 0x060018C5 RID: 6341 RVA: 0x00064B58 File Offset: 0x00062D58
        public void ScrollPercentY(float p)
        {
            Vector3 localPosition = this.Inner.transform.localPosition;
            localPosition.y = this.ContentYBounds.Lerp(p);
            this.Inner.transform.localPosition = localPosition;
            this.UpdateScrollBars();
        }

        // Token: 0x060018C6 RID: 6342 RVA: 0x00064BA0 File Offset: 0x00062DA0
        public void ScrollPercentX(float p)
        {

            Vector3 localPosition = this.Inner.transform.localPosition;
            localPosition.x = this.ContentXBounds.Lerp(p);
            currentScroll = localPosition;
            this.Inner.transform.localPosition = -currentScroll;
            this.UpdateScrollBars();
        }
        public void ScrollPostX(float p)
        {

            Vector3 localPosition = this.Inner.transform.localPosition;
            localPosition.x = p;
            currentScroll = localPosition;
            this.Inner.transform.localPosition = -currentScroll;
            this.UpdateScrollBars();
        }

        // Token: 0x060018C7 RID: 6343 RVA: 0x00064BE8 File Offset: 0x00062DE8
        public override void ReceiveClickDown()
        {
            this.active = true;
        }

        // Token: 0x060018C8 RID: 6344 RVA: 0x00064BF1 File Offset: 0x00062DF1
        public override void ReceiveClickUp()
        {
            this.active = false;
        }

        // Token: 0x060018C9 RID: 6345 RVA: 0x00064BFA File Offset: 0x00062DFA
        public override void ReceiveClickDrag(Vector2 dragDelta)
        {
            this.velocity = dragDelta / Time.deltaTime * 0.9f;
            this.ScrollRelative(dragDelta);
        }

        // Token: 0x060018CA RID: 6346 RVA: 0x00064C20 File Offset: 0x00062E20
        public void ScrollToScrollbarPositionY(Vector3 newScrollbarPosition)
        {
            newScrollbarPosition.y = this.ScrollbarYBounds.Clamp(newScrollbarPosition.y);
            float p = 1f - this.ScrollbarYBounds.ReverseLerp(newScrollbarPosition.y);
            this.ScrollPercentY(p);
        }

        // Token: 0x060018CB RID: 6347 RVA: 0x00064C64 File Offset: 0x00062E64
        public void ScrollToScrollbarPositionX(Vector3 newScrollbarPosition)
        {
            newScrollbarPosition.x = this.ScrollbarXBounds.Clamp(newScrollbarPosition.x);
            float p = 1f - this.ScrollbarXBounds.ReverseLerp(newScrollbarPosition.x);
            this.ScrollPercentX(p);
        }

        // Token: 0x060018CC RID: 6348 RVA: 0x00064CA8 File Offset: 0x00062EA8
        public void ScrollRelative(Vector2 dragDelta)
        {
            dragDelta *= this.DragScrollSpeed;

            if (!this.allowX)
                dragDelta.x = 0f;

            if (!this.allowY)
                dragDelta.y = 0f;

            dragDelta.x = -dragDelta.x;

            Vector3 vector = currentScroll + (Vector3)dragDelta;

            float maxY = Mathf.Max(this.ContentYBounds.min, this.ContentYBounds.max);
            vector.y = Mathf.Clamp(vector.y, this.ContentYBounds.min, maxY);

            float maxX = Mathf.Max(this.ContentXBounds.min, this.ContentXBounds.max);
            vector.x = Mathf.Clamp( vector.x, this.ContentXBounds.min, maxX);

            currentScroll = vector;
            this.Inner.transform.localPosition = -currentScroll;
            this.UpdateScrollBars();
        }


        // Token: 0x060018CD RID: 6349 RVA: 0x00064D94 File Offset: 0x00062F94
        public void UpdateScrollBars()
        {
            if (this.ScrollbarY)
            {
                if (!this.showY || this.ContentYBounds.min >= this.ContentYBounds.max)
                {
                    this.ScrollbarY.Toggle(false);
                }
                else
                {
                    this.ScrollbarY.Toggle(true);
                    float num = this.ContentYBounds.Lerp(this.Inner.transform.localPosition.y);
                    Vector3 localPosition = this.ScrollbarY.transform.localPosition;
                    localPosition.y = this.ScrollbarYBounds.ReverseLerp(1f - num);
                    this.ScrollbarY.UpdatePosition(localPosition);
                }
                Scroller.ScrollHandler onScrollYEvent = this.OnScrollYEvent;
                if (onScrollYEvent != null)
                {
                    onScrollYEvent.Invoke(this.GetScrollPercY());
                }
            }
            if (this.ScrollbarX)
            {
                if (!this.showX || this.ContentXBounds.min >= this.ContentXBounds.max)
                {
                    this.ScrollbarX.Toggle(false);
                }
                else
                {
                    this.ScrollbarX.Toggle(true);
                    float num2 = this.ContentXBounds.Lerp(this.Inner.transform.localPosition.x);
                    Vector3 localPosition2 = this.ScrollbarX.transform.localPosition;
                    localPosition2.x = this.ScrollbarXBounds.ReverseLerp(1f - num2);
                    this.ScrollbarX.UpdatePosition(localPosition2);
                }
                Scroller.ScrollHandler onScrollXEvent = this.OnScrollXEvent;
                if (onScrollXEvent == null)
                {
                    return;
                }
                onScrollXEvent.Invoke(this.GetScrollPercX());
            }
        }

        // Token: 0x060018CE RID: 6350 RVA: 0x00064F10 File Offset: 0x00063110
        public void ScrollToTop()
        {
            this.velocity = Vector2.zero;
            Vector3 localPosition = this.Inner.transform.localPosition;
            localPosition.y = this.ContentYBounds.min;
            currentScroll = localPosition;
            this.Inner.transform.localPosition = -currentScroll;
            this.UpdateScrollBars();
        }

        public Vector3 currentScroll = new Vector3(0,0,0);

        // Token: 0x040016AB RID: 5803
        public Transform Inner;
       
        // Token: 0x040016AC RID: 5804
        public bool allowY;

        // Token: 0x040016AD RID: 5805
        public bool showY = true;

        // Token: 0x040016AE RID: 5806
        public FloatRange ContentYBounds = new FloatRange(-10f, 10f);

        // Token: 0x040016AF RID: 5807
        public FloatRange ScrollbarYBounds;

        // Token: 0x040016B0 RID: 5808
        public Scrollbar ScrollbarY;

        // Token: 0x040016B1 RID: 5809
        public bool allowX;

        // Token: 0x040016B2 RID: 5810
        public bool showX = true;

        // Token: 0x040016B3 RID: 5811
        public FloatRange ContentXBounds = new FloatRange(-10f, 10f);

        // Token: 0x040016B4 RID: 5812
        public FloatRange ScrollbarXBounds;

        // Token: 0x040016B5 RID: 5813
        public Scrollbar ScrollbarX;

        // Token: 0x040016B6 RID: 5814
        public float DragScrollSpeed = 1f;

        // Token: 0x040016B7 RID: 5815
        public float ScrollWheelSpeed = 0.25f;

        // Token: 0x040016B8 RID: 5816
        public bool MouseMustBeOverToScroll;

        // Token: 0x040016B9 RID: 5817
        public Vector2 velocity;

        // Token: 0x040016BA RID: 5818
        public bool active;

        // Token: 0x040016BB RID: 5819
        public bool mouseOver;

        // Token: 0x040016BC RID: 5820
        public Scroller.ScrollHandler OnScrollXEvent;

        // Token: 0x040016BD RID: 5821
        public Scroller.ScrollHandler OnScrollYEvent;

        // Token: 0x020003EC RID: 1004
        // (Invoke) Token: 0x060018D1 RID: 6353
        public delegate void ScrollHandler(float value);
    }

}
